using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.YouTube
{
    /// <summary>
    /// Provides values for <see cref="YouTubeMetadata"/> and <see cref="YouTubeWatchPageData"/>.
    /// </summary>
    public class YouTubePageInfo
    {
        public YouTubePageInfo()
        {
        }

        public YouTubePageInfo(YouTubeWatchPageData page, YouTubeMetadata metadata)
        {
            Page = page;
            Metadata = metadata;
        }

        public YouTubeWatchPageData Page { get; set; }

        public YouTubeMetadata Metadata { get; set; }
    }
}
