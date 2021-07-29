using System;

namespace DotNetTools.SharpGrabber.Vimeo
{
    /// <summary>
    /// Provides extension methods for <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class VimeoGrabberBuilderExtensions
    {
        /// <summary>
        /// Includes the Vimeo grabber.
        /// </summary>
        public static IGrabberBuilder AddVimeo(this IGrabberBuilder builder)
            => builder.Add<VimeoGrabber>();
    }
}
