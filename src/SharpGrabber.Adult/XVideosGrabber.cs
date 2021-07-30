using System;
using System.Text.RegularExpressions;

namespace DotNetTools.SharpGrabber.Adult
{
    public class XVideosGrabber : XnxxGrabber
    {
        private static readonly Regex HostRegex = new(@"^(https?://)?(www\.)?xvideos.com/video([^/]+)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public XVideosGrabber(IGrabberServices services) : base(services)
        {
        }

        public override string StringId { get; } = "xvideos.com";

        public override string Name { get; } = "xvideos";

        public override bool Supports(Uri uri)
        {
            return GetVideoId(uri.ToString()) != null;
        }

        public static new string GetVideoId(string url)
        {
            var match = HostRegex.Match(url);
            if (match.Success)
                return match.Groups[3].Value;
            return null;
        }
    }
}
