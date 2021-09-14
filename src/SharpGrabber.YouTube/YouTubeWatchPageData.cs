using Newtonsoft.Json.Linq;
using System;

namespace DotNetTools.SharpGrabber.YouTube
{
    /// <summary>
    /// Contains data obtained from a YouTube watch/embedded page.
    /// </summary>
    public class YouTubeWatchPageData
    {
        /// <summary>
        /// Gets or sets the URI to YouTube base.js script.
        /// </summary>
        public Uri BaseJsUri { get; set; }

        /// <summary>
        /// Gets or sets the INNERTUBE_API_KEY.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the player response extracted from the page where available.
        /// </summary>
        public string RawPlayerResponse { get; set; }
    }
}