using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetTools.SharpGrabber.Hls
{
    public class PlaylistDocumentLoadException : Exception
    {
        public PlaylistDocumentLoadException() : this("Invalid M3U8 file.")
        {
        }

        public PlaylistDocumentLoadException(string message) : base(message)
        {
        }

        public PlaylistDocumentLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PlaylistDocumentLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
