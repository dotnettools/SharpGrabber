using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Exceptions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace DotNetTools.SharpGrabber.Internal.Grabbers
{
    /// <summary>
    /// Represents a YouTube <see cref="IGrabber"/>.
    /// </summary>
    public class YouTubeGrabber : BaseGrabber
    {
        #region YouTube link regular expressions
        private static readonly Regex YTUNormal = new Regex(
            @"(https?://)?(www\.)?youtube\.com/(watch|embed)\?v=([A-Za-z0-9\-_]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex YTUAlternative = new Regex(
            @"(https?://)?(www\.)?youtube\.com/(watch|embed)/([A-Za-z0-9\-_]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex YTUShort = new Regex(@"(https?://)?(www\.)?youtu\.be/([A-Za-z0-9\-_]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region Properties
        public override string Name { get; } = "YouTube";

        /// <summary>
        /// Standard format for YouTube links which is used with String.Format that is called with video ID as the only
        /// format argument
        /// </summary>
        public string StandardYouTubeUrlFormat { get; set; } = "http://www.youtube.com/watch?v={0}";
        #endregion

        #region Internal Methods
        /// <summary>
        /// Extracts YouTube video ID from the specified URI.
        /// </summary>
        protected virtual string ExtractVideoId(Uri uri)
        {
            // init
            var uriString = uri.ToString();
            var matchMap = new Dictionary<Regex, int>
            {
                {YTUNormal, 4},
                {YTUAlternative, 4},
                {YTUShort, 3},
            };

            // try match using each regular expression
            foreach (var matchPair in matchMap)
            {
                var match = matchPair.Key.Match(uriString);
                if (match.Success)
                    return match.Groups[matchPair.Value].Value;
            }

            return null;
        }

        private Uri MakeStandardYouTubeUri(string videoId) => new Uri(string.Format(StandardYouTubeUrlFormat, videoId));

        protected virtual void CheckResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new GrabException(
                    $"An HTTP error occurred while retrieving YouTube content. Server returned {response.StatusCode} {response.ReasonPhrase}.");
        }

        protected virtual JObject GetYTInitialData(string html)
        {
            var re = new Regex(@"ytInitialData[""=\]\s]+(\{.*\})[;\sA-Za-z0-9\[""]+ytInitialPlayerResponse",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var match = re.Match(html);
            if (!match.Success)
                throw new GrabParseException("Failed to fetch YouTube initial data.");
            return JObject.Parse(match.Groups[1].Value);
        }

        protected virtual async Task<GrabResult> Grab(HttpContent content)
        {
            // get html content
            var html = await content.ReadAsStringAsync();

            // extract javaScript info from the page
            var ytInitialData = GetYTInitialData(html);
            var ytResults = ytInitialData.SelectToken("$.contents.twoColumnWatchNextResults") ??
                            throw new GrabParseException("Failed to find twoColumnWatchNextResults.");


            return null;
        }
        #endregion

        #region Methods
        public override bool Supports(Uri uri) => !string.IsNullOrEmpty(ExtractVideoId(uri));

        public override async Task<GrabResult> Grab(Uri uri, GrabOptions options)
        {
            // extract id
            var id = ExtractVideoId(uri);
            if (string.IsNullOrEmpty(id))
                return null;

            // generate standard YouTube uri
            uri = MakeStandardYouTubeUri(id);

            // download target page
            Status.Update(null, "Downloading page...", WorkStatusType.DownloadingPage);
            var client = HttpHelper.CreateClient(uri);

            using (var response = await client.GetAsync(uri))
            {
                // check response
                CheckResponse(response);

                // grab from content
                return await Grab(response.Content);
            }
        }
        #endregion
    }
}