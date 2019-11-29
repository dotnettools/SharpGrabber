using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Internal.Grabbers.YouTube;

namespace DotNetTools.SharpGrabber.Internal.Grabbers
{
    /// <summary>
    /// Default <see cref="IGrabber"/> for YouTube
    /// </summary>
    public class YouTubeGrabber : YouTubeGrabberBase
    {
        #region Compiled Regular Expressions
        private static readonly Regex BaseJsLocatorRegex = new Regex(@"<script[^<>]+src=""([^""]+base\.js)""", RegexOptions.Compiled | RegexOptions.Multiline);
        #endregion

        #region Internal Methods
        /// <summary>
        /// Downloads video's embed page and extracts useful data.
        /// </summary>
        protected virtual async Task<YouTubeEmbedPageData> DownloadEmbedPage(string id)
        {
            var result = new YouTubeEmbedPageData();

            // get embed uri
            var embedUri = GetYouTubeEmbedUri(id);

            // download embed page
            var client = HttpHelper.CreateClient();
            var embedPageContent = await client.GetStringAsync(embedUri);

            // find base.js
            var match = BaseJsLocatorRegex.Match(embedPageContent);
            if (!match.Success)
                throw new GrabParseException("Failed to find base.js script reference.");
            result.BaseJsUri = new Uri(embedUri, match.Groups[1].Value);

            return result;
        }

        /// <summary>
        /// Downloads metadata for the YouTube video with the specified ID.
        /// </summary>
        protected virtual async Task<YouTubeMetadata> DownloadMetadata(string id, CancellationToken cancellationToken)
        {
            var rawMetadata = new Dictionary<string, string>();
            Status.Update(null, "Downloading metadata...", WorkStatusType.DownloadingFile);

            // make http client
            var client = HttpHelper.CreateClient();

            // send http request
            using (var response = await client.GetAsync(GetYouTubeVideoInfoUri(id), cancellationToken))
            {
                // decode metadata into rawMetadata
                var content = await response.Content.ReadAsStringAsync();
                var @params = content.Split('&');
                foreach (var param in @params)
                {
                    var pair = param.Split('=')
                        .Select(Uri.UnescapeDataString)
                        .ToArray();

                    rawMetadata.Add(pair[0], pair[1]);
                }
            }

            // extract metadata
            var metadata = new YouTubeMetadata
            {
            };
            return metadata;
        }

        protected override async Task GrabAsync(GrabResult result, string id, CancellationToken cancellationToken, GrabOptions options)
        {
            // extract base.js script
            var embedPageData = await DownloadEmbedPage(id);

            // download metadata
            var metaData = await DownloadMetadata(id, cancellationToken);

            throw new NotImplementedException();
        }
        #endregion
    }
}