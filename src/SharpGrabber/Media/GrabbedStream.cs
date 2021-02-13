using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber.Media
{
    public class GrabbedStream : IGrabbed
    {
        public GrabbedStream(Uri originalUri, Uri resourceUri, TimeSpan length, IEnumerable<MediaSegment> segments)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
            Length = length;
            Segments = segments.ToList().AsReadOnly();
        }

        public Uri OriginalUri { get; }

        public Uri ResourceUri { get; }

        /// <summary>
        /// Total duration of the stream
        /// </summary>
        public TimeSpan Length { get; }

        /// <summary>
        /// Segments of the stream
        /// </summary>
        public IReadOnlyList<MediaSegment> Segments { get; }
    }
}
