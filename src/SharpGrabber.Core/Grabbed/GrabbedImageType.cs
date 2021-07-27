using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Defined role of an image grabbed from a page.
    /// </summary>
    public enum GrabbedImageType
    {
        /// <summary>
        /// Indicates that the image is one of the main images (or the only image) of the target media.
        /// </summary>
        Primary,

        /// <summary>
        /// Indicates that the image is a thumbnail.
        /// </summary>
        Thumbnail,

        /// <summary>
        /// Indicates that the image is a frame of a video. Using this image type requires specification of 
        /// the exact time of the frame.
        /// </summary>
        Frame,

        /// <summary>
        /// Indicates that the image is a preview of the media.
        /// </summary>
        Preview,
    }
}
