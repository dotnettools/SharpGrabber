using DotNetTools.SharpGrabber.YouTube;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Provides extension methods for <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class YouTubeGrabberBuilderExtensions
    {
        /// <summary>
        /// Includes the YouTube grabber.
        /// </summary>
        public static IGrabberBuilder AddYouTube(this IGrabberBuilder builder)
        {
            return builder.Add<YouTubeGrabber>();
        }
    }
}
