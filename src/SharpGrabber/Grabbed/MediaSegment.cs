using System;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Represents a single segment of a <see cref="GrabbedHlsStream"/>.
    /// </summary>
    public class MediaSegment
    {
        public MediaSegment(string title, Uri uri, TimeSpan? duration)
        {
            Title = title;
            Uri = uri;
            Duration = duration;
        }

        /// <summary>
        /// Optional title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Absolute URI of the media segment
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Optional duration of the segment
        /// </summary>
        public TimeSpan? Duration { get; }
    }
}
