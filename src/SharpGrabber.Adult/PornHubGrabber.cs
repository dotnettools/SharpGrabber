using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetTools.SharpGrabber.Exceptions;
using System.Linq;
using System.Collections.Generic;
using DotNetTools.SharpGrabber.Hls;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Internal;
using HtmlAgilityPack;
using Jint.Native.Object;
using System.Dynamic;

namespace DotNetTools.SharpGrabber.Adult
{
    /// <summary>
    /// Represents a PornHub.com grabber.
    /// </summary>
    public class PornHubGrabber : GrabberBase
    {
        private static readonly Regex UrlMatcher = new(@"^(https?://)?(www\.)?pornhub\.com/([^/]+)viewkey=(\w+).*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex FlashVarsFinder = new(@"\s*(var|let)\s+(flashvars[\w_]+)\s+=",
            RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public PornHubGrabber(IGrabberServices services) : base(services)
        {
        }

        public override string StringId { get; } = "pornhub.com";

        /// <inheritdoc />
        public override string Name { get; } = "PornHub";

        /// <summary>
        /// Will participate in a call to <see cref="string.Format(string, object)"/> with video 'viewkey' as argument.
        /// </summary>
        public string StandardUriFormat { get; set; } = "https://www.pornhub.com/view_video.php?viewkey={0}";

        /// <inheritdoc />
        public override bool Supports(Uri uri) => GetViewId(uri) != null;

        /// <inheritdoc />
        protected override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress)
        {
            // grab view id
            var viewId = GetViewId(uri);
            if (viewId == null)
                return null;
            uri = MakeStandardUri(viewId);

            var client = Services.GetClient();

            // download content
            var response = await client.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var htmlContent = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var flashVars = ParseFlashVarsScript(doc);
            if (flashVars == null)
                throw new GrabParseException("Failed to locate flashvars.");
            var grabbedList = new List<IGrabbed>();
            var result = new GrabResult(uri, grabbedList);
            Grab(result, grabbedList, flashVars, options);
            return result;
        }

        private static string GetViewId(string uriString)
        {
            var match = UrlMatcher.Match(uriString);
            return !match.Success ? null : match.Groups[4].Value;
        }

        private static string GetViewId(Uri uri) => GetViewId(uri.ToString());

        private Uri MakeStandardUri(string viewId) => new Uri(string.Format(StandardUriFormat, viewId));

        private IDictionary<string, object> ParseFlashVarsScript(HtmlDocument doc)
        {
            string source = null, varName = null;
            foreach (var scriptNode in doc.DocumentNode.SelectNodes("//script"))
            {
                var match = FlashVarsFinder.Match(scriptNode.InnerText);
                if (match.Success)
                {
                    source = scriptNode.InnerText;
                    varName = match.Groups[2].Value;
                    break;
                }
            }
            if (source == null)
                throw new GrabParseException("Failed to parse flash vars.");
            var engine = new Engine();
            var result = engine.Evaluate($"let playerObjList = {{}};{source};return {varName};").As<ObjectInstance>();
            var expando = result.ToObject() as ExpandoObject;
            return expando.ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task<(JObject flashVars, JArray qualityItems)> ExtractFlashVars(string script,
            string flashVarsVariableName, string qualityItemsVariableName)
        {
            return await Task.Run(() =>
            {
                var engine = new Engine();
                engine.Execute(script);
                var vars = engine.GetValue(flashVarsVariableName)?.ToObject();
                if (vars == null)
                    throw new GrabParseException("Failed to fetch flashvars variable.");
                var qualityVars = engine.GetValue(qualityItemsVariableName)?.ToObject();
                if (qualityVars == null)
                    throw new GrabParseException("Failed to fetch qualityItems variable.");

                var flashVarsObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(vars)) as JObject;
                var qualityItemsObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(qualityVars)) as JArray;

                return (flashVarsObject, qualityItemsObject);
            });
        }

        private static readonly MediaFormat DefaultMediaFormat = new MediaFormat("video/mp4", "mp4");

        protected virtual void Grab(GrabResult result, List<IGrabbed> resources, IDictionary<string, object> flashVars, GrabOptions options)
        {
            var grabbed = new Dictionary<int, GrabbedMedia>();

            if (options.Flags.HasFlag(GrabOptionFlags.GrabImages))
            {
                var image_url = new Uri(result.OriginalUri, flashVars["image_url"] as string);
                resources.Add(new GrabbedImage(GrabbedImageType.Primary, image_url));
            }

            result.Title = flashVars["video_title"] as string;

            resources.Add(new GrabbedInfo
            {
                Length = TimeSpan.FromSeconds(Convert.ToInt32(flashVars["video_duration"]))
            });

            var mediaDefinitions = (flashVars["mediaDefinitions"] as IEnumerable<object>)
                .OfType<ExpandoObject>()
                .Select(d => d.ToDictionary(x => x.Key, x => x.Value))
                .ToArray();
            foreach (var def in mediaDefinitions)
            {
                var format = def["format"] as string;
                var url = def["videoUrl"] as string;
                var isQualityArr = def["quality"] is IEnumerable<object>;
                var qualityArr = isQualityArr ? (def["quality"] as IEnumerable<object>).Select(o => Convert.ToInt32(o)).ToArray() : null;
                var quality = isQualityArr ? 0 : StringHelper.ForceParseInt(def["quality"] as string);
                if (grabbed.ContainsKey(quality) || string.IsNullOrEmpty(url))
                    continue;
                if (isQualityArr && qualityArr.Length == 0)
                    continue;
                var uri = new Uri(result.OriginalUri, url);
                var resol = isQualityArr ? null : $"{quality}p";
                var playlistType = isQualityArr ? HlsPlaylistType.Master : HlsPlaylistType.Stream;

                switch (format.ToLowerInvariant())
                {
                    case "mp4":
                        var m = new GrabbedMedia(uri, DefaultMediaFormat, MediaChannels.Both)
                        {
                            Resolution = resol,
                            FormatTitle = $"MP4 {resol}",
                        };
                        grabbed.Add(quality, m);
                        break;
                    case "hls":
                        var sr = new GrabbedHlsStreamReference(uri)
                        {
                            Resolution = resol,
                            PlaylistType = playlistType,
                        };
                        resources.Add(sr);
                        break;
                    default:
                        continue;
                }
            }

            foreach (var g in grabbed.OrderByDescending(m => m.Key))
                resources.Add(g.Value);
        }
    }
}