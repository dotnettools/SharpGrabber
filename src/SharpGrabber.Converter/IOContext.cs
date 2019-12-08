using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Converter
{
    unsafe sealed class IOContext : IDisposable
    {
        #region Fields
        private AVIOContext* _ioContext;
        private bool usedAvioOpen;
        #endregion

        #region Properties
        public AVIOContext* Pointer => _ioContext;
        #endregion

        #region Constructor
        public IOContext(AVIOContext* ioContext)
        {
            _ioContext = ioContext;
        }

        public IOContext(string path, int flags)
        {
            usedAvioOpen = true;
            AVIOContext* ioContext = null;
            ffmpeg.avio_open2(&ioContext, path, flags, null, null).ThrowOnError();
            _ioContext = ioContext;
        }

        public IOContext(Uri uri, int flags) : this(uri.IsFile ? uri.LocalPath : uri.ToString(), flags) { }
        #endregion

        #region Methods
        public void Dispose()
        {
            if (_ioContext != null)
            {
                var ioContext = _ioContext;
                if (usedAvioOpen)
                    ffmpeg.avio_close(ioContext);
                else
                    ffmpeg.avio_context_free(&ioContext);
                _ioContext = null;
            }
        }
        #endregion
    }
}
