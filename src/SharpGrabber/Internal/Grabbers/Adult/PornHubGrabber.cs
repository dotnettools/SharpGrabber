using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Media;
using System.Linq;
using System.Collections.Generic;
using DotNetTools.SharpGrabber.Hls;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.Adult
{
    /// <summary>
    /// Represents a PornHub.com grabber.
    /// </summary>
    public class PornHubGrabber : BaseGrabber
    {
        private static readonly Regex UrlMatcher = new Regex(@"^(https?://)?(www\.)?pornhub\.com/([^/]+)viewkey=(\w+).*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex FlashVarsFinder = new Regex(@"((var|let)\s+(flashvars[\w_]+)(.|[\r\n])+)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex QualityItemsVarFinder = new Regex(@"((var|let)\s+(qualityItems[\w_]+)(.|[\r\n])+)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <inheritdoc />
        public override string Name { get; } = "PornHub";

        /// <summary>
        /// Will participate in a call to <see cref="string.Format(string, object)"/> with video 'viewkey' as argument.
        /// </summary>
        public string StandardUriFormat { get; set; } = "https://www.pornhub.com/view_video.php?viewkey={0}";

        /// <inheritdoc />
        public override bool Supports(Uri uri) => GetViewId(uri) != null;

        /// <inheritdoc />
        public override async Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options)
        {
            // grab view id
            var viewId = GetViewId(uri);
            if (viewId == null)
                return null;
            uri = MakeStandardUri(viewId);

            using var client = HttpHelper.CreateClient();
            // download content
            var response = await client.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var htmlContent = await response.Content.ReadAsStringAsync();

            // cut the useful part of htmlContent to speed up regex look up
            htmlContent = CutUsefulPart(htmlContent);

            var objListIndex = htmlContent.IndexOf("playerObjList.");
            if (objListIndex < 0)
                throw new GrabParseException("Could not find the video.");
            htmlContent = htmlContent.Insert(objListIndex, "var playerObjList = {};\r\n");

            // grab javascript flashvars
            var flashVarsMatch = FlashVarsFinder.Match(htmlContent);
            if (!flashVarsMatch.Success)
                throw new GrabParseException("Failed to locate flashvars.");
            var qualityVarsMatch = QualityItemsVarFinder.Match(htmlContent);
            if (!qualityVarsMatch.Success)
                throw new GrabParseException("Failed to locate qualityItems.");
            var flashVarsVariableName = flashVarsMatch.Groups[3].Value;
            var qualityItemsVariableName = qualityVarsMatch.Groups[3].Value;
            var (flashVars, qualityItems) = await ExtractFlashVars(flashVarsMatch.Groups[1].Value, flashVarsVariableName,
                qualityItemsVariableName);

            // generate result
            var result = new GrabResult(uri);
            Grab(result, flashVars, qualityItems, options);
            return result;
        }

        private static string GetViewId(string uriString)
        {
            var match = UrlMatcher.Match(uriString);
            return !match.Success ? null : match.Groups[4].Value;
        }

        private static string GetViewId(Uri uri) => GetViewId(uri.ToString());

        private Uri MakeStandardUri(string viewId) => new Uri(string.Format(StandardUriFormat, viewId));

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

        protected virtual void Grab(GrabResult result, JObject flashVars, JArray qualityItemVars, GrabOptions options)
        {
            var grabbed = new Dictionary<int, GrabbedMedia>();

            if (options.Flags.HasFlag(GrabOptionFlag.GrabImages))
            {
                var image_url = new Uri(result.OriginalUri, flashVars.SelectToken("$.image_url").Value<string>());
                result.Resources.Add(new GrabbedImage(GrabbedImageType.Primary, null, image_url));
            }

            result.Title = flashVars.SelectToken("$.video_title").Value<string>();
            result.Statistics = new GrabStatisticInfo
            {
                Length = TimeSpan.FromSeconds(flashVars.SelectToken("$.video_duration").Value<int>())
            };

            if (qualityItemVars != null && qualityItemVars.Count > 0)
            {
                foreach (var quality in qualityItemVars)
                {
                    var url = quality.Value<string>("url");
                    if (string.IsNullOrEmpty(url))
                        continue;
                    var vid = new GrabbedMedia(new Uri(result.OriginalUri, url), result.OriginalUri, DefaultMediaFormat, MediaChannels.Both);
                    vid.Resolution = quality.Value<string>("text");
                    var qint = StringHelper.ForceParseInt(vid.Resolution);
                    grabbed.Add(qint, vid);
                }
            }

            var mediaDefinitions = flashVars.SelectToken("$.mediaDefinitions");
            foreach (var def in mediaDefinitions)
            {
                var format = def.Value<string>("format");
                var url = def.Value<string>("videoUrl");
                var isQualityArr = def["quality"] is JArray;
                var qualityArr = isQualityArr ? def["quality"].Values<int>().ToArray() : null;
                var quality = isQualityArr ? 0 : StringHelper.ForceParseInt(def.Value<string>("quality"));
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
                        var m = new GrabbedMedia(uri, result.OriginalUri, DefaultMediaFormat, MediaChannels.Both)
                        {
                            Resolution = resol,
                            FormatTitle = $"MP4 {resol}",
                        };
                        grabbed.Add(quality, m);
                        break;
                    case "hls":
                        var sr = new GrabbedStreamReference(uri, result.OriginalUri)
                        {
                            Resolution = resol,
                            PlaylistType = playlistType,
                        };
                        result.Resources.Add(sr);
                        break;
                    default:
                        continue;
                }
            }

            foreach (var g in grabbed.OrderByDescending(m => m.Key))
                result.Resources.Add(g.Value);
        }

        /// <summary>
        /// Accepts the whole source of target page and returns only the part that contains video player script.
        /// </summary>
        protected virtual string CutUsefulPart(string htmlContent)
        {
            void CheckIndex(int index)
            {
                if (index < 0)
                    throw new GrabParseException("Failed to find the script containing flashvars.");
            }

            var from = htmlContent.IndexOf("flashvars_", StringComparison.OrdinalIgnoreCase);
            CheckIndex(from);
            from = htmlContent.LastIndexOf("<script", from, StringComparison.OrdinalIgnoreCase);
            CheckIndex(from);
            from = htmlContent.IndexOf(">", from) + 1;
            var to = htmlContent.IndexOf("</script>", from, StringComparison.OrdinalIgnoreCase);
            CheckIndex(to);
            return htmlContent.Substring(from, to - from).Trim();
        }
    }
}