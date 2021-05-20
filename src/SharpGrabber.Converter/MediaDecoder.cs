using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using FFmpeg.AutoGen;

namespace DotNetTools.SharpGrabber.Converter
{
    public sealed unsafe class MediaDecoder : IDisposable
    {
        #region Fields
        private AVFormatContext* _avFormatContext;
        private AVCodecContext* _avCodecContext;
        private int _streamIndex;
        #endregion

        #region Properties
        public Uri Source { get; }

        public AVHWDeviceType HardwareDevice { get; }

        public AVCodecID CodecId { get; private set; }

        public string CodecName { get; private set; }

        public long BitRate { get; private set; }

        public AVRational FrameRate { get; private set; }

        public AVRational TimeBase { get; private set; }

        public Size FrameSize { get; private set; }

        public int AudioFrameSize { get; private set; }

        public AVPixelFormat PixelFormat { get; private set; }

        public AVCodecContext* CodecContext => _avCodecContext;
        #endregion

        #region Constructor
        public MediaDecoder(Uri source, AVHWDeviceType hardwareDevice)
        {
            Source = source;
            HardwareDevice = hardwareDevice;
            Open();
        }
        #endregion

        #region Internal Methods
        private void Open()
        {
            string path = Source.IsFile ? Source.LocalPath : Source.ToString();
            AVFormatContext* avFormatContext;
            ffmpeg.avformat_open_input(&avFormatContext, path, null, null).ThrowOnError();
            ffmpeg.avformat_find_stream_info(avFormatContext, null).ThrowOnError();
            _avFormatContext = avFormatContext;
        }
        #endregion

        #region Methods
        public bool SelectStream(AVMediaType type)
        {
            AVCodec* avCodec = null;
            _streamIndex = ffmpeg.av_find_best_stream(_avFormatContext, type, -1, -1, &avCodec, 0);
            if (_streamIndex == ffmpeg.AVERROR_STREAM_NOT_FOUND)
                return false;
            _streamIndex.ThrowOnError();
            _avCodecContext = ffmpeg.avcodec_alloc_context3(avCodec);
            var stream = _avFormatContext->streams[_streamIndex];

            if (HardwareDevice != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
                ffmpeg.av_hwdevice_ctx_create(&_avCodecContext->hw_device_ctx, HardwareDevice, null, null, 0).ThrowOnError();

            ffmpeg.avcodec_parameters_to_context(_avCodecContext, stream->codecpar).ThrowOnError();
            ffmpeg.avcodec_open2(_avCodecContext, avCodec, null).ThrowOnError();

            CodecId = avCodec->id;
            CodecName = ffmpeg.avcodec_get_name(CodecId);
            FrameSize = new Size(_avCodecContext->width, _avCodecContext->height);
            AudioFrameSize = _avCodecContext->frame_size;
            PixelFormat = HardwareDevice == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE ? _avCodecContext->pix_fmt : GetHWPixelFormat(HardwareDevice);
            BitRate = _avCodecContext->bit_rate;
            FrameRate = _avCodecContext->framerate;
            TimeBase = stream->time_base;

            return true;
        }

        public AVStream* GetStream()
        {
            return _avFormatContext->streams[_streamIndex];
        }

        public void Dispose()
        {
            if (_avFormatContext != null)
            {
                var avFormatContext = _avFormatContext;
                if (Source.IsFile)
                    ffmpeg.avformat_close_input(&avFormatContext);
                else
                    ffmpeg.avformat_free_context(avFormatContext);
                _avFormatContext = null;
            }

            if (_avCodecContext != null)
            {
                var avCodecContext = _avCodecContext;
                ffmpeg.avcodec_free_context(&avCodecContext);
                _avCodecContext = null;
            }
        }

        private bool ReadFrame(AVPacket* packet)
        {
            do
            {
                var result = ffmpeg.av_read_frame(_avFormatContext, packet);
                if (result == ffmpeg.AVERROR_EOF)
                    return false;
                result.ThrowOnError();
            } while (packet->stream_index != _streamIndex);

            return true;
        }

        /// <summary>
        /// Tries to read the next packet. Returns NULL on EOF.
        /// </summary>
        public MediaPacket ReadPacket()
        {
            var packet = ffmpeg.av_packet_alloc();

            try
            {
                ffmpeg.av_init_packet(packet);

                if (!ReadFrame(packet))
                    return null;

                return new MediaPacket(packet);
            }
            catch (Exception)
            {
                ffmpeg.av_packet_unref(packet);
                throw;
            }
        }

        //public MediaFrame ReadFrame(MediaPacket packet)
        //{
        //    var frame = ffmpeg.av_frame_alloc();
        //    ffmpeg.avcodec_send_packet(_avCodecContext, packet.Pointer).ThrowOnError();

        //    while (true)
        //    {
        //        var result = ffmpeg.avcodec_receive_frame(_avCodecContext, frame).ThrowOnError();
        //        if (result == ffmpeg.AVERROR(ffmpeg.EAGAIN))
        //        {
        //            ReadFrame(packet.Pointer);
        //            continue;
        //        }
        //        result.ThrowOnError();
        //        break;
        //    };

        //    // hardware decode
        //    if (HardwareDevice != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
        //    {
        //        var hwframe = ffmpeg.av_frame_alloc();
        //        ffmpeg.av_hwframe_transfer_data(hwframe, frame, 0).ThrowOnError();
        //        ffmpeg.av_frame_unref(frame);
        //        frame = hwframe;
        //    }

        //    return new MediaFrame(frame);
        //}

        //public MediaFrame ReadFrame()
        //{
        //    using var packet = ReadPacket();
        //    if (packet == null)
        //        return null;
        //    return ReadFrame(packet);
        //}

        //public bool ReadFrameAndConvert(Action<AVFrame, IntPtr> frameFeed, AVPixelFormat pixelFormat = AVPixelFormat.AV_PIX_FMT_BGR24)
        //{
        //    using var frame = ReadFrame();
        //    if (frame == null)
        //        return false;

        //    using var conf = new VideoFrameConverter(FrameSize, PixelFormat, FrameSize, pixelFormat);
        //    var convFrame = conf.Convert(*frame.Pointer);
        //    frameFeed.Invoke(convFrame, (IntPtr)convFrame.data[0]);
        //    return true;
        //}
        #endregion

        #region Static Methods
        private static AVPixelFormat GetHWPixelFormat(AVHWDeviceType hWDevice)
        {
            switch (hWDevice)
            {
                case AVHWDeviceType.AV_HWDEVICE_TYPE_NONE:
                    return AVPixelFormat.AV_PIX_FMT_NONE;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VDPAU:
                    return AVPixelFormat.AV_PIX_FMT_VDPAU;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA:
                    return AVPixelFormat.AV_PIX_FMT_CUDA;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VAAPI:
                    return AVPixelFormat.AV_PIX_FMT_VAAPI;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2:
                    return AVPixelFormat.AV_PIX_FMT_NV12;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_QSV:
                    return AVPixelFormat.AV_PIX_FMT_QSV;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_VIDEOTOOLBOX:
                    return AVPixelFormat.AV_PIX_FMT_VIDEOTOOLBOX;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA:
                    return AVPixelFormat.AV_PIX_FMT_NV12;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_DRM:
                    return AVPixelFormat.AV_PIX_FMT_DRM_PRIME;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_OPENCL:
                    return AVPixelFormat.AV_PIX_FMT_OPENCL;
                case AVHWDeviceType.AV_HWDEVICE_TYPE_MEDIACODEC:
                    return AVPixelFormat.AV_PIX_FMT_MEDIACODEC;
                default:
                    return AVPixelFormat.AV_PIX_FMT_NONE;
            }
        }
        #endregion
    }
}
