namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Represents an adaptive YouTube stream.
    /// Information about an adaptive stream is obtained from adaptive_fmts.
    /// </summary>
    public class YouTubeAdaptiveStream : YouTubeStreamInfo
    {
        /// <summary>
        /// Name of this quality suggested by YouTube e.g. mp4 720p
        /// </summary>
        public string QualityLabel { get; set; }

        /// <summary>
        /// Frame per second
        /// </summary>
        public int FPS { get; set; }

        /// <summary>
        /// Audio Bit Rate / 1000 e.g. 48, which stands for 48k (audio only)
        /// </summary>
        public int BitRate { get; set; }

        /// <summary>
        /// Video frame size (video only)
        /// </summary>
        public Size Size { get; set; }
    }
}