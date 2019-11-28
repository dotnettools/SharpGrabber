using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Media
{
    /// <summary>
    /// Represents grabbed images
    /// </summary>
    public class GrabbedImage : IGrabbed
    {
        #region Properties
        public Uri OriginalUri { get; }

        public Uri ResourceUri { get; }

        /// <summary>
        /// Type of the grabbed image e.g. thumbnail, frame etc.
        /// </summary>
        public GrabbedImageType Type { get; }

        /// <summary>
        /// Time of the frame - if <see cref="Type"/> is <see cref="GrabbedImageType.Frame"/>
        /// </summary>
        public TimeSpan? FrameTime { get; set; }

        /// <summary>
        /// Size of the image - if available
        /// </summary>
        public GrabbedImageSize Size { get; set; }
        #endregion

        #region Constructors
        public GrabbedImage(GrabbedImageType type, Uri originalUri, Uri resourceUri)
        {
            Type = type;
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
        }
        #endregion
    }
}