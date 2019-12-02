using System;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Contains data obtained from a YouTube embedded page.
    /// </summary>
    public class YouTubeEmbedPageData
    {
        #region Properties
        /// <summary>
        /// Link to YouTube base.js script
        /// </summary>
        public Uri BaseJsUri { get; set; }
        #endregion
    }
}