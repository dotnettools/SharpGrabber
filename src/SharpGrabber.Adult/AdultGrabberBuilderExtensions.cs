using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Adult
{
    /// <summary>
    /// Provides extension methods for <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class AdultGrabberBuilderExtensions
    {
        /// <summary>
        /// Includes the PornHub grabber.
        /// </summary>
        public static IGrabberBuilder AddPornHub(this IGrabberBuilder builder)
            => builder.Add<PornHubGrabber>();

        /// <summary>
        /// Includes the XNXX grabber.
        /// </summary>
        public static IGrabberBuilder AddXnxx(this IGrabberBuilder builder)
            => builder.Add<XnxxGrabber>();

        /// <summary>
        /// Includes the xvideos grabber.
        /// </summary>
        public static IGrabberBuilder AddXVideos(this IGrabberBuilder builder)
            => builder.Add<XVideosGrabber>();
    }
}
