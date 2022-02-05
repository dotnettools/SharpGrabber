using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Auth;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Grabbed;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Helpers;

namespace DotNetTools.SharpGrabber.Instagram
{
    /// <summary>
    /// Represents an Instagram <see cref="IGrabber"/>.
    /// </summary>
    public class InstagramGrabber : GrabberBase
    {
        private readonly Regex _idPattern =
            new Regex(@"^https?://(www\.)?instagram\.com/(\w+)/([A-Za-z0-9_-]+)(/.*)?$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly Regex _graphqlScriptRegex = new(@"<script[^<>]+>([^<>]+)graphql([^<>]+)</script>");
        private readonly Func<IInstaApi> _instagramApiFactory;

        public InstagramGrabber(Func<IInstaApi> instagramApiFactory, IGrabberServices services) : base(services)
        {
            _instagramApiFactory = instagramApiFactory ?? (() => InstaApiBuilder.CreateBuilder()
               // Omitted .UseHttpClient(services.GetClient())
               .Build());
            // Note: When building the Instagram API client, we cannot use our own HttpClient, because of a bug in InstagramApiSharp that
            // makes it dependent on HttpClient base URI.
        }

        public InstagramGrabber(IGrabberServices services) : this(null, services)
        {
        }

        public override string StringId { get; } = "instagram.com";

        /// <inheritdoc />
        public override string Name { get; } = "Instagram";

        /// <summary>
        /// Gets or sets whether to try fetching the Instagram post without authentication first.
        /// Default is TRUE.
        /// </summary>
        public bool TryAsGuest { get; } = true;

        /// <summary>
        /// Represents the template of standard Instagram links. This value will be formatted using <see cref="string.Format(string, object[])"/>
        /// passing an Instagram post identifier containing alpha-numeric characters.
        /// </summary>
        public virtual string StandardUrlTemplate { get; set; } = "https://www.instagram.com/p/{0}/";


        /// <inheritdoc />
        public override bool Supports(Uri uri) => ParseUrl(uri).HasValue;

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
            var client = Services.GetClient();
            var api = _instagramApiFactory();

            if (!api.IsUserAuthenticated)
            {
                if (TryAsGuest)
                {
                    var result = await GrabAsGuestAsync(uri, client, cancellationToken).ConfigureAwait(false);
                    if (result != null)
                        return result;
                }

                var authState = new InstagramAuthenticationRequestState(api);
                var authRequest = new GrabberAuthenticationRequest(this, authState, cancellationToken);
                await Services.Authentication.RequestAsync(authRequest).ConfigureAwait(false);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes a standard Instagram URL using the given post ID.
        /// </summary>
        protected virtual Uri MakeStandardInstagramUri(string id) => new Uri(string.Format(StandardUrlTemplate, id));

        /// <summary>
        /// Extracts post ID from the specified URI.
        /// </summary>
        protected virtual InstagramUrlDescriptor? ParseUrl(Uri uri)
        {
            var uriString = uri.ToString();
            var match = _idPattern.Match(uriString);
            if (!match.Success)
                return null;
            return new InstagramUrlDescriptor(match.Groups[2].Value, match.Groups[3].Value);
        }

        /// <summary>
        /// Extracts post ID from the specified URI.
        /// </summary>
        protected string GrabId(Uri uri)
        {
            var desc = ParseUrl(uri);
            return desc?.ContentId;
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

            var match = _graphqlScriptRegex.Match(content);
            if (!match.Success)
                // Failed to obtain metadata from the Instagram page
                return null;

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

        private async Task<GrabResult> GrabAsGuestAsync(Uri uri, HttpClient client, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            // check response
            CheckResponse(response);

            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            // parse page
            var meta = ParsePage(responseStream);
            if (meta == null)
                return null;

            // parse pairs
            return GrabUsingMetadata(meta);
        }

        /// <summary>
        /// Describes segments of an Instagram URL.
        /// </summary>
        public readonly struct InstagramUrlDescriptor
        {
            /// <summary>
            /// Instagram content type e.g. reel, tv etc.
            /// </summary>
            public readonly string Type;

            /// <summary>
            /// Instagram content ID
            /// </summary>
            public readonly string ContentId;

            public InstagramUrlDescriptor(string type, string contentId)
            {
                Type = type;
                ContentId = contentId;
            }
        }
    }
}