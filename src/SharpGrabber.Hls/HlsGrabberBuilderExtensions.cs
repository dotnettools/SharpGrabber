using System;
using System.Collections.Generic;
using System.Text;
using DotNetTools.SharpGrabber.Hls;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Provides extension methods for <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class HlsGrabberBuilderExtensions
    {
        /// <summary>
        /// Includes HLS grabber.
        /// </summary>
        public static IGrabberBuilder AddHls(this IGrabberBuilder builder)
            => builder.Add<HlsGrabber>();
    }
}
