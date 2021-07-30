using DotNetTools.SharpGrabber.Adult.Internal;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Hls;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Adult
{
    public class XnxxGrabber : GrabberBase
    {
        private static readonly Regex HostRegex = new Regex(@"^(https?://)?(www\.)?xnxx.com/video-([^/]+)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex VideoVarNameRegex = new Regex(@"var\s+(\w+)\s+=\s+new\s+HTML5Player", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly string VideoParamRegexString = @"{0}\.(set)?(\w+)\(([^\(\)]+)\)";

        public XnxxGrabber(IGrabberServices services) : base(services)
        {
        }


        public override string Name { get; } = "XNXX";

        protected override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress)
        {
            if (!Supports(uri))
                return null;

            var client = Services.GetClient();
            var content = await client.GetStringAsync(uri).ConfigureAwait(false);

            var resources = new List<IGrabbed>();
            var result = new GrabResult(uri, resources);
            var paramMap = ParsePage(content, result);

            // grab info
            result.Title = paramMap.GetOrDefault("title")?.ToString();
            resources.Add(new GrabbedInfo
            {
                Length = TimeSpan.FromSeconds(Convert.ToDouble(paramMap.GetOrDefault("duration"))),
            });

            // grab images
            var img = (paramMap.GetOrDefault("image") ?? paramMap.GetOrDefault("ThumbUrl169") ?? paramMap.GetOrDefault("ThumbUrl")) as string;
            if (Uri.TryCreate(img, UriKind.Absolute, out var imgUri))
                resources.Add(new GrabbedImage(GrabbedImageType.Thumbnail, uri, imgUri));
            img = (paramMap.GetOrDefault("ThumbSlideBig") ?? paramMap.GetOrDefault("ThumbSlide")) as string;
            if (Uri.TryCreate(img, UriKind.Absolute, out imgUri))
                resources.Add(new GrabbedImage(GrabbedImageType.Preview, uri, imgUri));

            // grab resources
            var hls = paramMap["VideoHLS"] as string;
            if (string.IsNullOrEmpty(hls))
                throw new GrabParseException("Could not locate the HLS playlist file.");
            var hlsUri = new Uri(hls);
            var hlsGrabber = new HlsGrabber(Services);
            var hlsGrabResult = await hlsGrabber.GrabAsync(hlsUri, cancellationToken, options).ConfigureAwait(false);
            foreach (var resource in hlsGrabResult.Resources)
                resources.Add(resource);

            return result;
        }

        public override bool Supports(Uri uri)
        {
            return GetVideoId(uri.ToString()) != null;
        }

        public static string GetVideoId(string url)
        {
            var match = HostRegex.Match(url);
            if (match.Success)
                return match.Groups[3].Value;
            return null;
        }

        private IDictionary<string, object> ParsePage(string content, GrabResult result)
        {
            var varNameMatch = VideoVarNameRegex.Match(content);
            if (!varNameMatch.Success)
                throw new GrabParseException("Failed to locate the video variable in the script.");
            var varName = varNameMatch.Groups[1].Value;

            var videoParamRegex = new Regex(string.Format(VideoParamRegexString, varName), RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var videoParamMatches = videoParamRegex.Matches(content);

            var paramMap = new Dictionary<string, object>();
            foreach (Match paramMatch in videoParamMatches)
            {
                var name = paramMatch.Groups[2].Value;
                var stringValue = paramMatch.Groups[3].Value.Replace('\'', '"');

                if (JsonHelper.TryParseJson(stringValue, out var value))
                    paramMap.Add(name, value);
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var metaTags = doc.DocumentNode.SelectNodes("//meta[starts-with(@property,'og:')]");
            foreach (var tag in metaTags)
            {
                var name = tag.GetAttributeValue("property", null).Replace("og:", string.Empty);
                var val = tag.GetAttributeValue("content", null);
                paramMap.Add(name, val);
            }
            return paramMap;
        }

        private class PageInfo
        {
            public IDictionary<string, object> Params { get; set; }
        }
    }
}
