namespace DotNetTools.SharpGrabber.YouTube
{
    /// <summary>
    /// Represents an adaptive YouTube stream.
    /// Information about an adaptive stream is obtained from adaptive_fmts.
    /// </summary>
    public class YouTubeAdaptiveStream : YouTubeStreamInfo
    {
        /// <summary>
        /// Frame per second
        /// </summary>
        public int FPS { get; set; }
    }
}