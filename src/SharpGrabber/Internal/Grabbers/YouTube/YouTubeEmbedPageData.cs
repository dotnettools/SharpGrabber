using System;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Contains data obtained from a YouTube embedded page.
    /// </summary>
    public class YouTubeEmbedPageData
    {
        /// <summary>
        /// Gets or sets the URI to YouTube base.js script.
        /// </summary>
        public Uri BaseJsUri { get; set; }

        /// <summary>
        /// Gets or sets the INNERTUBE_API_KEY.
        /// </summary>
        public string Key { get; set; }
    }
}