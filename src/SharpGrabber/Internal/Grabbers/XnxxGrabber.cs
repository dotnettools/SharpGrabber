using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Internal.Grabbers.Hls;
using DotNetTools.SharpGrabber.Media;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Internal.Grabbers
{
    public class XnxxGrabber : BaseGrabber
    {
        private static readonly Regex HostRegex = new Regex(@"^(https?://)?(www\.)?xnxx.com/video-([^/]+)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string Name { get; } = "XNXX";

        public override async Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options)
        {
            if (!Supports(uri))
                return null;

            using var client = HttpHelper.CreateClient(uri);
            var content = await client.GetStringAsync(uri).ConfigureAwait(false);

            var result = new GrabResult(uri);
            var paramMap = ParsePage(content, result);

            // grab info
            result.Title = paramMap.GetOrDefault("title")?.ToString();
            result.Statistics = new GrabStatisticInfo
            {
                Length = TimeSpan.FromSeconds(Convert.ToDouble(paramMap.GetOrDefault("duration"))),
            };

            // grab images
            var img = (paramMap.GetOrDefault("image") ?? paramMap.GetOrDefault("ThumbUrl169") ?? paramMap.GetOrDefault("ThumbUrl")) as string;
            if (Uri.TryCreate(img, UriKind.Absolute, out var imgUri))
                result.Resources.Add(new GrabbedImage(GrabbedImageType.Thumbnail, uri, imgUri));
            img = (paramMap.GetOrDefault("ThumbSlideBig") ?? paramMap.GetOrDefault("ThumbSlide")) as string;
            if (Uri.TryCreate(img, UriKind.Absolute, out imgUri))
                result.Resources.Add(new GrabbedImage(GrabbedImageType.Preview, uri, imgUri));

            // grab resources
            var hls = paramMap["VideoHLS"] as string;
            if (string.IsNullOrEmpty(hls))
                throw new GrabParseException("Could not locate the HLS playlist file.");
            var hlsUri = new Uri(hls);
            var hlsGrabber = new HlsGrabber();
            var hlsGrabResult = await hlsGrabber.GrabAsync(hlsUri, cancellationToken, options).ConfigureAwait(false);
            foreach (var resource in hlsGrabResult.Resources)
                result.Resources.Add(resource);

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

        private static readonly Regex VideoVarNameRegex = new Regex(@"var\s+(\w+)\s+=\s+new\s+HTML5Player", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly string VideoParamRegexString = @"{0}\.(set)?(\w+)\(([^\(\)]+)\)";

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
