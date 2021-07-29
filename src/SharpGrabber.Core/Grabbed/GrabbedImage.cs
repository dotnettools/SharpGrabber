using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Represents grabbed images
    /// </summary>
    public class GrabbedImage : IGrabbed
    {

        public GrabbedImage(GrabbedImageType type, Uri originalUri, Uri resourceUri)
        {
            Type = type;
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
        }


        /// <inheritdoc />
        public Uri OriginalUri { get; }

        /// <inheritdoc />
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
    }
}