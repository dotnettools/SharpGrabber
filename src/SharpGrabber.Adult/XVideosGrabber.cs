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

namespace DotNetTools.SharpGrabber.Adult
{
    public class XVideosGrabber : XnxxGrabber
    {
        private static readonly Regex HostRegex = new Regex(@"^(https?://)?(www\.)?xvideos.com/video([^/]+)/", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
