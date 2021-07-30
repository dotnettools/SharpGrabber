using FFmpeg.AutoGen;
using System;

namespace DotNetTools.SharpGrabber.Converter
{
    /// <summary>
    /// Wrapper for <see cref="AVFrame"/>.
    /// </summary>
    public sealed unsafe class MediaFrame : IDisposable
    {
        #region Properties
        public AVFrame* Pointer { get; private set; }
        #endregion

        #region Constructor
        public MediaFrame(AVFrame* frame)
        {
            Pointer = frame;
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            if (Pointer != null)
            {
                var frame = Pointer;
                ffmpeg.av_frame_unref(frame);
                Pointer = null;
            }
        }
        #endregion
    }
}
