using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Vimeo.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Vimeo
{
    /// <summary>
    /// Implements <see cref="IGrabber"/> for Vimeo.
    /// </summary>
    public partial class VimeoGrabber : GrabberBase
    {
        private readonly Regex _idPattern =
            new Regex(@"^https?://(www\.|player\.)?vimeo\.com/(video/)?([0-9]+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public VimeoGrabber(IGrabberServices services) : base(services)
        {
        }

        public override string StringId { get; } = "vimeo.com";

        public override string Name { get; } = "Vimeo";

        public override bool Supports(Uri uri) => !string.IsNullOrEmpty(GrabId(uri));

        protected override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress)
        {
            // init
            var id = GrabId(uri);
            if (id == null)
                return null;

            var configUri = new Uri($"https://player.vimeo.com/video/{id}/config");

            // download target page
            var client = Services.GetClient();
            var response = await client.GetAsync(configUri, cancellationToken);

            // check response
            CheckResponse(response);

            // parse the config
            var config = await GetConfigurationAsync(response.Content);

            // create the grab result
            return GrabUsingConfiguration(config, uri);
        }

        /// <summary>
        /// Extracts post ID from the specified URI.
        /// </summary>
        protected virtual string GrabId(Uri uri)
        {
            var uriString = uri.ToString();
            var match = _idPattern.Match(uriString);
            if (!match.Success)
                return null;
            return match.Groups[3].Value;
        }

        /// <summary>
        /// Validates response of the page request. For example, in the beginning the returned status code is checked.
        /// </summary>
        protected virtual void CheckResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new GrabException(
                    $"An HTTP error occurred while retrieving Vimeo content. Server returned {response.StatusCode} {response.ReasonPhrase}.");
        }

        /// <summary>
        /// This utility method get's the corresponding configuration for this video.
        /// </summary>
        private static async Task<VimeoConfiguration> GetConfigurationAsync(HttpContent content)
        {
            var result = await content.ReadAsStringAsync();
            var configuration = JsonConvert.DeserializeObject<VimeoConfiguration>(result);

            return configuration;
        }

        /// <summary>
        /// Given the specified <paramref name="configuration"/>, generates proper <see cref="GrabResult"/>.
        /// </summary>
        protected virtual GrabResult GrabUsingConfiguration(VimeoConfiguration configuration, Uri originalUri)
        {
            var grabList = new List<IGrabbed>();

            // Ensure we have at least one video format available
            if (string.IsNullOrWhiteSpace(configuration?.Request?.Files?.Progressive?.FirstOrDefault().Url))
                return null;

            if (configuration.Video?.Thumbs != null)
            {
                foreach (var keyValuePair in configuration.Video.Thumbs)
                {
                    var imageType = GrabbedImageType.Thumbnail;
                    if (keyValuePair.Key == "base")
                        imageType = GrabbedImageType.Primary;

                    grabList.Add(new GrabbedImage(imageType, new Uri(keyValuePair.Value)));
                }
            }

            foreach (var progressive in configuration.Request.Files.Progressive)
            {
                var format = new MediaFormat(progressive.Mime, Services.Mime.ExtractMimeExtension(progressive.Mime));
                var vid = new GrabbedMedia(new Uri(progressive.Url), format, MediaChannels.Both)
                {
                    Resolution = progressive.Quality
                };
                grabList.Add(vid);
            }

            var result = new GrabResult(originalUri, grabList)
            {
                Title = configuration.Video?.Title
            };
            return result;
        }
    }
}