using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotNetTools.SharpGrabber.YouTube.YouTube
{
    /// <summary>
    /// Base class for YouTube <see cref="IGrabber"/>s
    /// </summary>
    public abstract class YouTubeGrabberBase : GrabberBase
    {
        #region Constants
        /// <summary>
        /// Standard format for YouTube image links which is used with String.Format provided with the following arguments:
        /// <list type="bullet">
        /// <item>{0} Protocol Schema e.g. https</item>
        /// <item>{1} Video ID</item>
        /// <item>{2} Image name e.g. hqdefault</item>
        /// </list>
        /// </summary>
        private static string StandardYouTubeImageUrlFormat { get; set; } = "{0}://img.youtube.com/vi/{1}/{2}.jpg";
        #endregion

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

        #region Constructors
        protected YouTubeGrabberBase(IGrabberServices services) : base(services)
        {
        }
        #endregion

        #region Properties
        /// <inheritdoc />
        public override string Name { get; } = "YouTube";

        /// <summary>
        /// Standard format for YouTube links which is used with String.Format supplied with video ID as the only
        /// format argument.
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
        [Obsolete("The get_video_info API no longer works")]
        protected virtual Uri GetYouTubeVideoInfoUri(string videoId)
        {
            var eUrl = $"https://youtube.googleapis.com/v/{videoId}";

            var videoInfoUriTemplate = "https://youtube.com/get_video_info?video_id={0}&eurl=https%3A%2F%2Fyoutube.googleapis.com%2Fv%2F{0}&html5=1&c=TVHTML5&cver=6.20180913";

            return new Uri(string.Format(videoInfoUriTemplate, videoId, Uri.EscapeDataString(eUrl)));
        }

        protected virtual Task<HttpResponseMessage> GetYouTubeVideoInfoResponse(HttpClient client, string videoId, string key,
            CancellationToken cancellationToken)
        {
            var uri = new Uri($"https://www.youtube.com/youtubei/v1/player?key={Uri.EscapeDataString(key)}");
            var body = new
            {
                context = new
                {
                    client = new
                    {
                        hl = "en",
                        clientName = "WEB",
                        clientVersion = "2.20210721.00.00",
                        clientFormFactor = "UNKNOWN_FORM_FACTOR",
                        clientScreen = "WATCH",
                        mainAppWebInfo = new
                        {
                            graftUrl = $"/watch?v={videoId}",
                        }
                    },
                    user = new
                    {
                        lockedSafetyMode = false,
                    },
                    request = new
                    {
                        useSsl = true,
                        internalExperimentFlags = Array.Empty<object>(),
                        consistencyTokenJars = Array.Empty<object>()
                    }
                },
                videoId = videoId,
                playbackContext = new
                {
                    contentPlaybackContext = new
                    {
                        vis = 0,
                        splay = false,
                        autoCaptionsDefaultOn = false,
                        autonavState = "STATE_NONE",
                        html5Preference = "HTML5_PREF_WANTS",
                        lactMilliseconds = "-1",
                    }
                },
                racyCheckOk = false,
                contentCheckOk = false
            };
            return client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, MimeType.Json),
                cancellationToken);
        }

        /// <summary>
        /// Creates video's embed page URI.
        /// </summary>
        protected virtual Uri GetYouTubeEmbedUri(string videoId) => new Uri($"https://youtube.com/embed/{videoId}");

        /// <summary>
        /// This method gets called internally by <see cref="GrabAsync"/> after necessary initializations.
        /// </summary>
        protected abstract Task GrabAsync(GrabResult result, IList<IGrabbed> resources,
            string id, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress);
        #endregion

        #region Methods
        /// <inheritdoc />
        public override bool Supports(Uri uri) => !string.IsNullOrEmpty(GetYouTubeId(uri));

        /// <inheritdoc />
        protected sealed override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress)
        {
            // get YouTube ID
            var id = GetYouTubeId(uri);
            if (string.IsNullOrEmpty(id))
                return null;

            // generate original link
            var originalUri = GetYouTubeStandardUri(id);

            // prepare result
            var resources = new List<IGrabbed>();
            var result = new GrabResult(originalUri, resources);

            // grab using the internal grab method
            await GrabAsync(result, resources, id, cancellationToken, options, progress).ConfigureAwait(false);

            return result;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns URI for the specified YouTube image.  
        /// </summary>
        public static Uri GetYouTubeImageUri(string videoId, YouTubeImageType type, bool useHttps = true)
            => new Uri(string.Format(StandardYouTubeImageUrlFormat,
                useHttps ? "https" : "http",
                videoId,
                type.ToYouTubeString()));
        #endregion
    }
}