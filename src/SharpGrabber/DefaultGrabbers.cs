using DotNetTools.SharpGrabber.Internal.Grabbers;
using DotNetTools.SharpGrabber.Internal.Grabbers.Hls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    public static class DefaultGrabbers
    {
        public static IReadOnlyCollection<Type> Types { get; }

        static DefaultGrabbers()
        {
            Types = new[] {
                typeof(YouTubeGrabber),
                typeof(VimeoGrabber),
                typeof(HlsGrabber),
                typeof(XnxxGrabber),
            }.ToList().AsReadOnly();
        }
    }
}
