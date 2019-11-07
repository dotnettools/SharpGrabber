using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Media
{
    /// <summary>
    /// <see cref="IGrabbed"/> for media resources
    /// </summary>
    public class GrabbedMedia : IGrabbed
    {
        #region Properties
        public Uri OriginalUri { get; }

        public Uri ResourceUri { get; }

        public MediaFormat Format { get; }

        /// <summary>
        /// Defines if this grabbed media is audio, video, or image.
        /// </summary>
        public MediaType Type { get; }

        /// <summary>
        /// Full title obtained from source
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Length of the media - might be unavailable for some resources
        /// </summary>
        public TimeSpan? Length { get; set; }
        #endregion

        #region Constructors
        public GrabbedMedia(Uri resourceUri, Uri originalUri, MediaFormat format, MediaType type)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
            Format = format;
            Type = type;
        }
        #endregion
    }
}
