using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DotNetTools.SharpGrabber.Exceptions;

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

        protected virtual int[] ExtractCreationDateTime(JToken ytPrimary)
        {
            var ytDateText = ytPrimary.SelectToken("$.dateText.simpleText") ??
                             throw new GrabParseException("Failed to extract dateText.simpleText.");
            var ytDateMatch = new Regex(@"^\s*([0-9]+)[^0-9]+([0-9]+)[^0-9]+([0-9]+)\s*$")
                .Match(ytDateText.Value<string>());
            if (!ytDateMatch.Success)
                throw new GrabParseException("Failed to parse date format of dateText.simpleText.");
            return new[]
            {
                int.Parse(ytDateMatch.Groups[1].Value), int.Parse(ytDateMatch.Groups[2].Value),
                int.Parse(ytDateMatch.Groups[3].Value)
            };
        }

        protected virtual async Task Grab(GrabResult result, HttpContent content)
        {
            // get html content
            var html = await content.ReadAsStringAsync();

            // extract javaScript info from the page
            var ytInitialData = GetYTInitialData(html);
            var ytResults = ytInitialData.SelectToken("$.contents.twoColumnWatchNextResults") ??
                            throw new GrabParseException("Failed to extract twoColumnWatchNextResults.");
            var ytPrimary = ytResults.SelectToken("$.results.results.contents[*].videoPrimaryInfoRenderer") ??
                            throw new GrabParseException("Failed to extract videoPrimaryInfoRenderer.");
            var ytSecondary = ytResults.SelectToken("$.results.results.contents[*].videoSecondaryInfoRenderer") ??
                              throw new GrabParseException("Failed to extract videoSecondaryInfoRenderer.");

            var ytDateValues = ExtractCreationDateTime(ytPrimary);

            // extract useful info from the JSON data
            var ytText = ytPrimary.SelectToken("$.title.runs[*].text");
            var ytDescription = ytSecondary.SelectTokens("$.description.runs[*].text") ??
                                throw new GrabParseException("Failed to extract description.");

            // update grab result
            result.Title = ytText.Value<string>();
            result.Description = string.Join(Environment.NewLine, ytDescription.Select(run => run.Value<string>()));
            result.CreationDate = new DateTime(ytDateValues[2], ytDateValues[1], ytDateValues[0]);
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
            Status.Update(null, WorkStatusType.DownloadingPage);
            var client = HttpHelper.CreateClient(uri);

            using (var response = await client.GetAsync(uri))
            {
                // check response
                CheckResponse(response);

                // grab from content
                var result = new GrabResult(uri);
                await Grab(result, response.Content);
                return result;
            }
        }
        #endregion
    }
}