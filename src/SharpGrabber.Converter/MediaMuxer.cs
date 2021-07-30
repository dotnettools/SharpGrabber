using FFmpeg.AutoGen;
using System;

namespace DotNetTools.SharpGrabber.Converter
{
    sealed unsafe class MediaMuxer : IDisposable
    {
        #region Fields
        private AVFormatContext* _formatContext;
        private IOContext _ioContext;
        #endregion

        #region Properties
        public AVFormatContext* FormatContextPtr => _formatContext;
        #endregion

        #region Constructors
        public MediaMuxer(IOContext output, string shortName, string mimeType)
        {
            _ioContext = output;
            var formatContext = _formatContext;
            var format = ffmpeg.av_guess_format(shortName, null, mimeType);
            ffmpeg.avformat_alloc_output_context2(&formatContext, format, null, null).ThrowOnError();
            _formatContext = formatContext;
            _formatContext->pb = _ioContext.Pointer;
        }
        #endregion

        #region Methods
        public AVStream* AddStream(AVCodec* codec)
        {
            var stream = ffmpeg.avformat_new_stream(_formatContext, codec);
            if (stream == null)
                throw new Exception("Could not allocate stream.");
            if ((_formatContext->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) > 0)
                stream->codec->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
            return stream;
        }

        public void WriteHeader()
        {
            ffmpeg.avformat_write_header(_formatContext, null).ThrowOnError();
        }

        public void WritePacket(AVPacket* packet)
        {
            ffmpeg.av_interleaved_write_frame(_formatContext, packet).ThrowOnError();
        }

        public static MediaPacket EncodeFrame(AVCodecContext* codecContext, MediaFrame mediaFrame)
        {
            var frame = mediaFrame.Pointer;
            var packet = ffmpeg.av_packet_alloc();
            ffmpeg.av_init_packet(packet);

            ffmpeg.avcodec_send_frame(codecContext, frame).ThrowOnError();
            ffmpeg.avcodec_receive_packet(codecContext, packet).ThrowOnError();

            return new MediaPacket(packet);
        }

        public void WriteFrame(AVCodecContext* codecContext, MediaFrame mediaFrame)
        {
            using var packet = EncodeFrame(codecContext, mediaFrame);
            WritePacket(packet.Pointer);
        }

        public void WriteTrailer()
        {
            ffmpeg.av_write_trailer(_formatContext).ThrowOnError();
        }

        public void Dispose()
        {
            if (_formatContext != null)
            {
                ffmpeg.avformat_free_context(_formatContext);
                _formatContext = null;
            }
            if (_ioContext != null)
            {
                _ioContext.Dispose();
                _ioContext = null;
            }
        }
        #endregion
    }
}
