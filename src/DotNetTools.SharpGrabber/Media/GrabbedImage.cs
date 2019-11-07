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
