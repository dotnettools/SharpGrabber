using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Describes a grabbed HLS stream.
    /// </summary>
    [GrabbedType("HlsStream")]
    public class GrabbedHlsStream : IGrabbedResource
    {
        public GrabbedHlsStream() { }

        [Obsolete("Use the parameterless constructor instead.")]
        public GrabbedHlsStream(Uri originalUri, Uri resourceUri, TimeSpan length, IEnumerable<MediaSegment> segments)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
            Length = length;
            Segments = segments.ToList().AsReadOnly();
        }

        public Uri OriginalUri { get; set; }

        public Uri ResourceUri { get; set; }

        /// <summary>
        /// Gets the total duration of the stream.
        /// </summary>
        public TimeSpan Length { get; set; }

        /// <summary>
        /// Gets the segments of the stream.
        /// </summary>
        public IReadOnlyList<MediaSegment> Segments { get; set; }
    }
}
