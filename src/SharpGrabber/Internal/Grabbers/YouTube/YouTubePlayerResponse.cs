using System;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// YouTube player response - Obtained from player_response
    /// </summary>
    public struct YouTubePlayerResponse
    {
        /// <summary>
        /// Video title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Video length
        /// </summary>
        public TimeSpan Length { get; set; }

        /// <summary>
        /// ID of the uploader channel
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// Short description of the video
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Average rating of the video out of 5
        /// </summary>
        public double AverageRating { get; set; }

        /// <summary>
        /// Total number of views
        /// </summary>
        public long ViewCount { get; set; }

        /// <summary>
        /// Author of the video
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Upload date
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// Publish date
        /// </summary>
        public DateTime PublishedAt { get; set; }
    }
}