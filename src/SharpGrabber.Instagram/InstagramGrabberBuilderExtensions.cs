using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Instagram
{
    /// <summary>
    /// Provides extension methods for <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class InstagramGrabberBuilderExtensions
    {
        /// <summary>
        /// Includes Instagram grabber.
        /// </summary>
        public static IGrabberBuilder AddInstagram(this IGrabberBuilder builder)
            => builder.Add<InstagramGrabber>();
    }
}
