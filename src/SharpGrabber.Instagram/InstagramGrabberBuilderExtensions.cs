using System;
using System.Collections.Generic;
using System.Text;
using DotNetTools.SharpGrabber.Instagram;

namespace DotNetTools.SharpGrabber
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
