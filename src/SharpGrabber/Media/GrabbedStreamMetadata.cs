using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Media
{
    /// <summary>
    /// <see cref="IGrabbed"/> for information about HLS master playlist streams. This class represents
    /// a stream, which refers to an M3U8 playlist containing the actual media files.
    /// </summary>
    public class GrabbedStreamMetadata : IGrabbed
    {
        public GrabbedStreamMetadata(Uri originalUri, Uri resourceUri, string name, Size resolution, int bandwidth,
            MediaFormat format, MediaFormat outputFormat, IResolvable<GrabbedStream> stream)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
            Name = name;
            Resolution = resolution;
            Bandwidth = bandwidth;
            StreamFormat = format;
            OutputFormat = outputFormat;
            Stream = stream;
        }

        public Uri OriginalUri { get; }

        public Uri ResourceUri { get; }

        /// <summary>
        /// Optional name for the stream
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Optional resolution of the stream
        /// </summary>
        public Size Resolution { get; }

        /// <summary>
        /// Bandwidth - or 0 if unknown
        /// </summary>
        public int Bandwidth { get; }

        /// <summary>
        /// Format of the stream file
        /// </summary>
        public MediaFormat StreamFormat { get; }

        /// <summary>
        /// Expected format of the output media
        /// </summary>
        public MediaFormat OutputFormat { get; }

        /// <summary>
        /// Resolves the stream associated with current metadata.
        /// </summary>
        public IResolvable<GrabbedStream> Stream { get; }
    }
}
