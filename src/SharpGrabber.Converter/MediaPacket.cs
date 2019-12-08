using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Converter
{
    /// <summary>
    /// Wrapper for <see cref="AVFrame"/>/
    /// </summary>
    public sealed unsafe class MediaPacket : IDisposable
    {
        #region Properties
        public AVPacket* Pointer { get; private set; }
        #endregion

        #region Constructor
        public MediaPacket(AVPacket* packet)
        {
            Pointer = packet;
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            if (Pointer != null)
            {
                var packet = Pointer;
                ffmpeg.av_packet_unref(packet);
                Pointer = null;
            }
        }
        #endregion
    }
}
