using System;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Represents a grabbed image.
    /// </summary>
    [GrabbedType("Image")]
    public class GrabbedImage : IGrabbedResource
    {
        public GrabbedImage()
        {
        }

        public GrabbedImage(GrabbedImageType type, Uri resourceUri)
        {
            Type = type;
            ResourceUri = resourceUri;
        }

        /// <inheritdoc />
        public Uri ResourceUri { get; set; }

        /// <summary>
        /// Type of the grabbed image e.g. thumbnail, frame etc.
        /// </summary>
        public GrabbedImageType Type { get; set; }

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