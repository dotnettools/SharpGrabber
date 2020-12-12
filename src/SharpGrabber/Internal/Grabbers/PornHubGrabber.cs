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

namespace DotNetTools.SharpGrabber.Internal.Grabbers
{
    /// <summary>
    /// Represents a PornHub.com grabber.
    /// </summary>
    public class PornHubGrabber : BaseGrabber
    {
        #region Compiled Regular Expressions
        private static readonly Regex UrlMatcher = new Regex(@"^(https?://)?(www\.)?pornhub\.com/([^/]+)viewkey=(\w+).*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex FlashVarsFinder = new Regex(@"((var|let)\s+(flashvars[\w_]+)(.|[\r\n])+)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Properties
        /// <inheritdoc />
        public override string Name { get; } = "PornHub";

        /// <summary>
        /// Will participate in a call to <see cref="string.Format(string, object)"/> with video 'viewkey' as argument.
        /// </summary>
        public string StandardUriFormat { get; set; } = "https://www.pornhub.com/view_video.php?viewkey={0}";
        #endregion

        #region Internal Methods
        private static string GetViewId(string uriString)
        {
            var match = UrlMatcher.Match(uriString);
            return !match.Success ? null : match.Groups[4].Value;
        }

        private static string GetViewId(Uri uri) => GetViewId(uri.ToString());

        private Uri MakeStandardUri(string viewId) => new Uri(string.Format(StandardUriFormat, viewId));

        private async Task<JObject> ExtractFlashVars(string script, string variableName)
        {
            return await Task.Run(() =>
            {
                var engine = new Engine();
                engine.Execute(script);
                var vars = engine.GetValue(variableName)?.ToObject();
                if (vars == null)
                    throw new GrabParseException("Failed to fetch flashvars object.");

                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(vars)) as JObject;
            });
        }

        protected virtual void Grab(GrabResult result, JObject vars, GrabOptions options)
        {
            if (options.Flags.HasFlag(GrabOptionFlag.GrabImages))
            {
                var image_url = new Uri(result.OriginalUri, vars.SelectToken("$.image_url").Value<string>());
                result.Resources.Add(new GrabbedImage(GrabbedImageType.Primary, null, image_url));
            }

            result.Title = vars.SelectToken("$.video_title").Value<string>();
            result.Statistics = new GrabStatisticInfo();
            result.Statistics.Length = TimeSpan.FromSeconds(vars.SelectToken("$.video_duration").Value<int>());
            var qualities = vars.SelectTokens("$.defaultQuality[*]").Select(t => t.Value<int>()).ToArray();

            foreach (var quality in qualities)
            {
                var key = $"quality_{quality}p";
                var url = vars.SelectToken($"$.{key}")?.Value<string>();
                if (string.IsNullOrEmpty(url))
                    continue;
                var vid = new GrabbedMedia(new Uri(result.OriginalUri, url), null, new MediaFormat("video/mp4", "mp4"), MediaChannels.Both);
                vid.Resolution = $"{quality}p";
                vid.FormatTitle = $"MP4 {vid.Resolution}";
                result.Resources.Add(vid);
            }
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
            var to = htmlContent.IndexOf("</script>", from, StringComparison.OrdinalIgnoreCase);
            CheckIndex(to);
            return htmlContent.Substring(from, to - from);
        }
        #endregion

        #region Methods
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

            using (var client = HttpHelper.CreateClient())
            {
                // download content
                var response = await client.GetAsync(uri, cancellationToken);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();

                // cut the useful part of htmlContent to speed up regex look up
                htmlContent = CutUsefulPart(htmlContent);

                htmlContent = htmlContent.Insert(htmlContent.IndexOf("playerObjList."), "var playerObjList = {};\r\n");

                // grab javascript flashvars
                var match = FlashVarsFinder.Match(htmlContent);
                if (!match.Success)
                    throw new GrabParseException("Failed to locate flashvars.");
                var variableName = match.Groups[3].Value;
                var flashVars = await ExtractFlashVars(match.Groups[1].Value, variableName);

                // generate result
                var result = new GrabResult(uri);
                Grab(result, flashVars, options);
                return result;
            }
        }
        #endregion
    }
}