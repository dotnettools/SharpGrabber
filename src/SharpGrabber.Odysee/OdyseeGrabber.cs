using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Grabbed;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DotNetTools.SharpGrabber.Odysee
{
    /// <summary>
    /// Implements <see cref="IGrabber"/> for the Odysee platform.
    /// </summary>
    public sealed class OdyseeGrabber : GrabberBase
    {
        private static readonly Regex UriRegex = new(
            @"^https?://(www\.)?odysee\.com/@([\w:]+)/([^\/]+)/?.*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public OdyseeGrabber(IGrabberServices services) : base(services)
        {
        }

        public override string StringId => "odysee.com";

        public override string Name => "Odysee";

        public override bool Supports(Uri uri)
        {
            return UriRegex.IsMatch(uri.ToString());
        }

        protected override async Task<GrabResult> InternalGrabAsync(Uri orgUri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            var urlInfo = ParseUrl(orgUri);
            var uri = urlInfo.ToUri();

            var client = Services.GetClient();
            using var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            var htmlContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            return GrabUsingHtml(doc, uri, options);
        }

        private GrabResult GrabUsingHtml(HtmlDocument doc, Uri uri, GrabOptions options)
        {
            var list = new List<IGrabbed>();

            var metaTags = new Dictionary<string, string?>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var tag in doc.DocumentNode.SelectNodes("//meta"))
            {
                var name = tag.GetAttributeValue<string?>("name", null) ?? tag.GetAttributeValue<string?>("property", null);
                var content = tag.GetAttributeValue("content", string.Empty);
                if (name == null)
                    continue;
                metaTags[name] = content;
            }

            var metadata = doc.DocumentNode.SelectNodes("//script")
                .Select(s => s.InnerHtml)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(JsonConvert.DeserializeObject)
                .OfType<JObject>()
                .Single();

            ExtractResources(list, metadata, options);

            return new GrabResult(uri, list)
            {
                Title = HttpUtility.HtmlDecode(metaTags.GetOrDefault("og:title")),
                Description = metaTags.GetOrDefault("og:description"),
                CreationDate = DateTime.Parse(metaTags.GetOrDefault("og:video:release_date")),
            };
        }

        private void ExtractResources(IList<IGrabbed> list, JObject metadata, GrabOptions options)
        {
            list.Add(new GrabbedInfo
            {
                Author = metadata["author"]?.Value<string>("name"),
                Length = XmlConvert.ToTimeSpan(metadata.Value<string>("duration")),
                ViewCount = null,
            });

            var thumbnail = metadata["thumbnail"];
            if (thumbnail != null)
                list.Add(new GrabbedImage(GrabbedImageType.Thumbnail,
                    new Uri(thumbnail.Value<string>("url"))));

            var contentUrl = metadata.Value<string>("contentUrl");
            if (contentUrl != null)
            {
                var ext = Path.GetExtension(contentUrl).TrimStart('.');
                list.Add(new GrabbedMedia
                {
                    Channels = MediaChannels.Both,
                    Container = ext,
                    PixelWidth = metadata.Value<int>("width"),
                    PixelHeight = metadata.Value<int>("height"),
                    ResourceUri = new Uri(contentUrl),
                    Format = new MediaFormat(Services.Mime.GetMimeByExtension(ext), ext)
                });
            }
        }

        private static OdyseeUrlInfo? TryParseUrl(Uri uri)
        {
            var match = UriRegex.Match(uri.ToString());
            if (!match.Success)
                return null;
            var authorId = Uri.UnescapeDataString(match.Groups[2].Value);
            var videoId = Uri.UnescapeDataString(match.Groups[3].Value);
            return new OdyseeUrlInfo(authorId, videoId);
        }

        private static OdyseeUrlInfo ParseUrl(Uri uri)
        {
            return TryParseUrl(uri) ?? throw new InvalidOperationException("The given URI is invalid for Odysee.");
        }

        private readonly struct OdyseeUrlInfo
        {
            public OdyseeUrlInfo(string authorId, string videoId)
            {
                AuthorId = authorId;
                VideoId = videoId;
            }

            public string AuthorId { get; }

            public string VideoId { get; }

            public Uri ToUri()
            {
                return new Uri($@"https://odysee.com/@{Uri.EscapeDataString(AuthorId)}/{Uri.EscapeDataString(VideoId)}");
            }
        }
    }
}
