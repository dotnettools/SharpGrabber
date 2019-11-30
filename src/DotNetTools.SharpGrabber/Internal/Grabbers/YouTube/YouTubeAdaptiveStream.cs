namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Represents an adaptive YouTube stream.
    /// Information about an adaptive stream is obtained from adaptive_fmts.
    /// </summary>
    public class YouTubeAdaptiveStream : YouTubeStreamInfo
    {
        public string QualityLabel { get; set; }

        public int FPS { get; set; }

        public int BitRate { get; set; }

        public Size Size { get; set; }
    }
}