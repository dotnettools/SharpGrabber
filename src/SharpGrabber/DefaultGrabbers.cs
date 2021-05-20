using DotNetTools.SharpGrabber.Internal.Grabbers;
using DotNetTools.SharpGrabber.Internal.Grabbers.Adult;
using DotNetTools.SharpGrabber.Internal.Grabbers.Hls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    public static class DefaultGrabbers
    {
        public static IReadOnlyCollection<Type> AllTypes { get; }
        public static IReadOnlyCollection<Type> NormalTypes { get; }
        public static IReadOnlyCollection<Type> AdultTypes { get; }

        static DefaultGrabbers()
        {
            NormalTypes = new[] {
                typeof(YouTubeGrabber),
                typeof(VimeoGrabber),
                typeof(HlsGrabber),
            }.ToList().AsReadOnly();

            AdultTypes = new[] {
                typeof(PornHubGrabber),
                typeof(XVideosGrabber),
                typeof(XnxxGrabber),
            }.ToList().AsReadOnly();

            AllTypes = NormalTypes.Union(AdultTypes).ToList().AsReadOnly();
        }
    }
}
