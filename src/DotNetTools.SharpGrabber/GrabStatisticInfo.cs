namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Contains statistics of a grabbed page e.g. Total views, comments, likes etc. for a YouTube video.
    /// </summary>
    /// <remarks>
    /// Each property of this class is optional/nullable, as they might not be applicable for different pages or
    /// grabbers.
    /// </remarks>
    public class GrabStatisticInfo
    {
        /// <summary>
        /// Total views of the page
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// Total number of comments on the page
        /// </summary>
        public long? CommentCount { get; set; }
    }
}