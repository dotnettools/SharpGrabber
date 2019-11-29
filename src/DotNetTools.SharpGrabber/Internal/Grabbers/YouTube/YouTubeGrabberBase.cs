using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Base class for YouTube <see cref="IGrabber"/>s
    /// </summary>
    public abstract class YouTubeGrabberBase : BaseGrabber
    {
        #region Compiled Regular Expressions
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
        public string StandardYouTubeUrlFormat { get; set; } = "https://www.youtube.com/watch?v={0}&hl=en_US";
        #endregion

        #region Internal Methods
        /// <summary>
        /// Extracts YouTube video ID from the specified URI.
        /// </summary>
        protected virtual string GetYouTubeId(Uri uri)
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

        /// <summary>
        /// Creates a standard YouTube URI of the specified YouTube video ID.
        /// </summary>
        protected virtual Uri GetYouTubeStandardUri(string videoId) => new Uri(string.Format(StandardYouTubeUrlFormat, videoId));

        /// <summary>
        /// Creates video info YouTube URI.
        /// </summary>
        protected virtual Uri GetYouTubeVideoInfoUri(string videoId)
        {
            var eUrl = $"https://youtube.googleapis.com/v/{videoId}";
            var videoInfoUriTemplate = "https://youtube.com/get_video_info?video_id={0}&eurl={1}&el=embedded&hl=en";

            return new Uri(string.Format(videoInfoUriTemplate, videoId, Uri.EscapeDataString(eUrl)));
        }

        /// <summary>
        /// Creates video's embed page URI.
        /// </summary>
        protected virtual Uri GetYouTubeEmbedUri(string videoId) => new Uri($"https://youtube.com/embed/{videoId}");

        /// <summary>
        /// This method gets called internally by <see cref="GrabAsync"/> after necessary initializations.
        /// </summary>
        protected abstract Task GrabAsync(GrabResult result, string id, CancellationToken cancellationToken, GrabOptions options);
        #endregion

        #region Methods
        public override bool Supports(Uri uri) => !string.IsNullOrEmpty(GetYouTubeId(uri));

        public sealed override async Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options)
        {
            // get YouTube ID
            var id = GetYouTubeId(uri);
            if (string.IsNullOrEmpty(id))
                return null;

            // generate original link
            var originalUri = GetYouTubeStandardUri(id);

            // prepare result
            var result = new GrabResult(originalUri);

            // grab using the internal grab method
            await GrabAsync(result, id, cancellationToken, options);

            return result;
        }
        #endregion
    }
}