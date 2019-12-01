using System;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// YouTube player response - Obtained from player_response
    /// </summary>
    public struct YouTubePlayerResponse
    {
        public string Title { get; set; }
        
        public TimeSpan Length { get; set; }
        
        public string ChannelId { get; set; }
        
        public string ShortDescription { get; set; }
        
        public double AverageRating { get; set; }
        
        public long ViewCount { get; set; }
        
        public string Author { get; set; }
        
        public DateTime UploadedAt { get; set; }
        
        public DateTime PublishedAt { get; set; }
    }
}