using System;

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
        /// Author name
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Total views of the page
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// Overall length of media if available
        /// </summary>
        public TimeSpan? Length { get; set; }
    }
}