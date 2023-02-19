using DotNetTools.SharpGrabber;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SharpGrabber.Odysee
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

            return GrabUsingHtml(doc, uri);
        }

        private GrabResult GrabUsingHtml(HtmlDocument doc, Uri uri)
        {
            var list = new List<IGrabbed>();

            var metaTags = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var tag in doc.DocumentNode.SelectNodes("//meta"))
            {
                var name = tag.GetAttributeValue<string?>("name", null) ?? tag.GetAttributeValue<string?>("property", null);
                var content = tag.GetAttributeValue("content", string.Empty);
                if (name == null)
                    continue;
                metaTags[name] = content;
            }

            doc.DocumentNode.SelectNodes("//script")
                .Select(s => s.InnerHtml)
                .Where(s => !string.IsNullOrEmpty(s));

            return new GrabResult(uri, list)
            {
                Title = metaTags.GetOrDefault("og:title"),
                Description = metaTags.GetOrDefault("og:description"),
                CreationDate = DateTime.Parse(metaTags.GetOrDefault("og:video:release_date")),
            };
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

        private readonly record struct OdyseeUrlInfo
        {
            public OdyseeUrlInfo()
            {
                AuthorId = VideoId = string.Empty;
            }

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
