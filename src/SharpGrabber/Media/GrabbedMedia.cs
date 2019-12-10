using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Media
{
    /// <summary>
    /// <see cref="IGrabbed"/> for media resources, such as audio and video.
    /// </summary>
    public class GrabbedMedia : IGrabbed
    {
        #region Properties
        /// <summary>
        /// Original URI - A value of NULL indicates inheritance from <see cref="GrabResult"/>.
        /// </summary>
        public Uri OriginalUri { get; }

        /// <inheritdoc />
        public Uri ResourceUri { get; }

        /// <summary>
        /// Optional unique identifier of this format - For YouTube media, this value would be the same as iTag integer number.
        /// </summary>
        /// <remarks>
        /// If specified, this value should be unique amongst all other grabbed media.
        /// </remarks>
        public object FormatId { get; set; }

        /// <summary>
        /// Contains information about mime type and extension of the media.
        /// </summary>
        public MediaFormat Format { get; }

        /// <summary>
        /// Defines if this grabbed media contains either audio and/or video channels.
        /// </summary>
        public MediaChannels Channels { get; }

        /// <summary>
        /// Full title obtained from source. A value of NULL indicates inheritance from <see cref="GrabResult"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Length of the media (optional)
        /// </summary>
        public TimeSpan? Length { get; set; }

        /// <summary>
        /// Content length (File size) of the target URI (optional)
        /// </summary>
        public long? ContentLength { get; set; }

        /// <summary>
        /// Media container name e.g. mp4 (optional)
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Media resolution in string format e.g. 720p (optional)
        /// </summary>
        public string Resolution { get; set; }

        /// <summary>
        /// Media bit rate in string format e.g. 48k (optional)
        /// </summary>
        public string BitRateString { get; set; }

        /// <summary>
        /// Suggested name that best describes this media format e.g. MP4 1080p
        /// <para>Can be optionally set by <see cref="IGrabber"/>s.</para>
        /// </summary>
        public string FormatTitle { get; set; }
        #endregion

        #region Constructors
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GrabbedMedia(Uri resourceUri, Uri originalUri, MediaFormat format, MediaChannels channels)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
            Format = format;
            Channels = channels;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion
    }
}