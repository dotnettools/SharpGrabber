using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Grabbed;

namespace DotNetTools.SharpGrabber.Instagram
{
    /// <summary>
    /// Represents an Instagram <see cref="IGrabber"/>.
    /// </summary>
    public class InstagramGrabber : GrabberBase
    {
        #region Fields

        private readonly Regex _idPattern =
            new Regex(@"^https?://(www\.)?instagram\.com/\w/([A-Za-z0-9_-]+)(/.*)?$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly Regex _graphqlScriptRegex = new Regex(@"<script[^<>]+>([^<>]+)graphql([^<>]+)</script>");
        #endregion

        #region Constructors
        public InstagramGrabber(IGrabberServices services) : base(services)
        {
        }
        #endregion

        #region Properties
        public override string StringId { get; } = "instagram.com";

        /// <inheritdoc />
        public override string Name { get; } = "Instagram";

        /// <summary>
        /// Represents the template of standard Instagram links. This value will be formatted using <see cref="string.Format(string, object[])"/>
        /// passing an Instagram post identifier containing alpha-numeric characters.
        /// </summary>
        public virtual string StandardUrlTemplate { get; set; } = "https://www.instagram.com/p/{0}/";

        #endregion

        #region Internal Methods

        /// <summary>
        /// Makes a standard Instagram URL using the given post ID.
        /// </summary>
        protected virtual Uri MakeStandardInstagramUri(string id) => new Uri(string.Format(StandardUrlTemplate, id));

        /// <summary>
        /// Extracts post ID from the specified URI.
        /// </summary>
        protected virtual string GrabId(Uri uri)
        {
            var uriString = uri.ToString();
            var match = _idPattern.Match(uriString);
            if (!match.Success)
                return null;
            return match.Groups[2].Value;
        }

        /// <summary>
        /// Validates response of the page request. For example, in the beginning the returned status code is checked.
        /// </summary>
        protected virtual void CheckResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new GrabException(
                    $"An HTTP error occurred while retrieving Instagram content. Server returned {response.StatusCode} {response.ReasonPhrase}.");
        }

        /// <summary>
        /// Parses content of the page and retrieves meta tags with useful information.
        /// </summary>
        protected virtual IDictionary<string, string> ParsePage(Stream responseStream)
        {
            var dictionary = new Dictionary<string, string>();

            string content;
            using (var reader = new StreamReader(responseStream))
                content = reader.ReadToEnd();

            Regex metaTagPattern = new Regex(@"<meta\s*property\s*=\s*""(?<propertyName>[^""]+)""\s*content\s*=\s*""(?<propertyValue>[^""]+)""");
            foreach (Match metaTagMatch in metaTagPattern.Matches(content))
            {
                dictionary.Add(metaTagMatch.Groups["propertyName"].Value, metaTagMatch.Groups["propertyValue"].Value);
            }
            // TODO: Instagram is changed and needs user to be authorized before accessing the content

            var match = _graphqlScriptRegex.Match(content);
            if (!match.Success)
                throw new GrabParseException("Failed to obtain metadata from the Instagram page.");

            return dictionary;
        }

        /// <summary>
        /// Given the specified <paramref name="metaData"/>, generates proper <see cref="GrabResult"/>.
        /// </summary>
        protected virtual GrabResult GrabUsingMetadata(IDictionary<string, string> metaData)
        {
            var grabList = new List<IGrabbed>();

            // extract metadata
            var title = metaData.GetOrDefault("og:title");
            var image = metaData.GetOrDefault("og:image");
            var description = metaData.GetOrDefault("og:description");
            var originalUri = new Uri(metaData.GetOrDefault("og:url"));
            var type = metaData.GetOrDefault("og:type");
            var video = metaData.GetOrDefault("og:video");
            var video_secure_url = metaData.GetOrDefault("og:video:secure_url");
            var video_type = metaData.GetOrDefault("og:video:type");
            var video_width = int.Parse(metaData.GetOrDefault("og:video:width", "0"));
            var video_height = int.Parse(metaData.GetOrDefault("og:video:height", "0"));

            // grab image
            if (!string.IsNullOrEmpty(image))
                grabList.Add(new GrabbedImage(GrabbedImageType.Primary, new Uri(image)));

            // grab video
            if (!string.IsNullOrEmpty(video))
            {
                var format = new MediaFormat(video_type, Services.Mime.ExtractMimeExtension(video_type));
                var vid = new GrabbedMedia(new Uri(video), format, MediaChannels.Both);
                grabList.Add(vid);
            }

            // make result
            var result = new GrabResult(originalUri, grabList)
            {
                Title = title,
                Description = description,
            };
            return result;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override bool Supports(Uri uri) => !string.IsNullOrEmpty(GrabId(uri));

        /// <inheritdoc />
        protected override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken,
            GrabOptions options, IProgress<double> progress)
        {
            // init
            var id = GrabId(uri);
            if (id == null)
                return null;

            // generate standard Instagram link
            uri = MakeStandardInstagramUri(id);

            // download target page
            var client = Services.GetClient();
            var response = await client.GetAsync(uri, cancellationToken);

            // check response
            CheckResponse(response);

            using var responseStream = await response.Content.ReadAsStreamAsync();
            // parse page
            var meta = ParsePage(responseStream);

            // parse pairs
            return GrabUsingMetadata(meta);
        }

        #endregion
    }
}