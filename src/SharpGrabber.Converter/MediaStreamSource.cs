using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Converter
{
    public enum MediaStreamType { Audio, Video }

    class MediaStreamSource
    {
        #region Properties
        public Uri Path { get; }

        public MediaStreamType StreamType { get; }
        #endregion

        #region Constructors
        public MediaStreamSource(Uri path, MediaStreamType streamType)
        {
            Path = path;
            StreamType = streamType;
        }
        #endregion
    }
}
