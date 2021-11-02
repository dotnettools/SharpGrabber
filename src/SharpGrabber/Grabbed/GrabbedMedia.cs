using System;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// <see cref="IGrabbed"/> for media resources, such as audio and video.
    /// </summary>
    [GrabbedType("Media")]
    public class GrabbedMedia : IGrabbedResource
    {
        public GrabbedMedia() { }

        public GrabbedMedia(Uri resourceUri, MediaFormat format, MediaChannels channels)
        {
            ResourceUri = resourceUri;
            Format = format ?? throw new ArgumentNullException(nameof(format));
            Channels = channels;
        }

        [Obsolete("Use the parameterless constructor instead.")]
        public GrabbedMedia(Uri resourceUri, Uri originalUri, MediaFormat format, MediaChannels channels)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
            Format = format ?? throw new ArgumentNullException(nameof(format));
            Channels = channels;
        }

        /// <summary>
        /// Original URI - A value of NULL indicates inheritance from <see cref="GrabResult"/>.
        /// </summary>
        public Uri OriginalUri { get; set; }

        /// <inheritdoc />
        public Uri ResourceUri { get; set; }

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
        public MediaFormat Format { get; set; }

        /// <summary>
        /// Defines if this grabbed media contains either audio and/or video channels.
        /// </summary>
        public MediaChannels Channels { get; set; }

        /// <summary>
        /// Full title obtained from source. A value of NULL indicates inheritance from <see cref="GrabResult"/>.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Duration of the media (optional)
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

        /// <summary>
        /// Gets the optional pixel width if <see cref="Channels"/> includes <see cref="MediaChannels.Video"/>.
        /// </summary>
        public int? PixelWidth { get; set; }

        /// <summary>
        /// Gets the optional pixel height if <see cref="Channels"/> includes <see cref="MediaChannels.Video"/>.
        /// </summary>
        public int? PixelHeight { get; set; }
    }
}