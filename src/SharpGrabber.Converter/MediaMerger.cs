using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetTools.SharpGrabber.Converter
{
    /// <summary>
    /// Can mux multiple media streams together into a container.
    /// </summary>
    public unsafe sealed class MediaMerger
    {
        #region Fields
        private readonly Dictionary<MediaStreamType, MediaStreamSource> _sources = new Dictionary<MediaStreamType, MediaStreamSource>();
        #endregion

        #region Properties
        /// <summary>
        /// Path to the output file
        /// </summary>
        public string OutputPath { get; set; }

        public string OutputShortName { get; set; } = "webm";

        public string OutputMimeType { get; set; } = "video/webm";

        public AVHWDeviceType HardwareDevice { get; set; }

        public AVCodecID? TargetAudioCodec { get; set; } = null;

        public AVCodecID? TargetVideoCodec { get; set; } = null;
        #endregion

        #region Constructor
        public MediaMerger() { }

        public MediaMerger(string outputPath)
        {
            OutputPath = outputPath;
        }
        #endregion

        #region Methods
        public void AddStreamSource(Uri path, MediaStreamType streamType)
        {
            _sources.Add(streamType, new MediaStreamSource(path, streamType));
        }

        public void AddStreamSource(string path, MediaStreamType streamType) => AddStreamSource(new Uri($"file://{path.Replace('\\', '/')}"), streamType);

        public void AutoSelectHardwareDevice()
        {
            var types = MediaHelper.GetHardwareDeviceTypes();
            if (types.Length > 0)
                HardwareDevice = types[0];
        }
        #endregion

        #region Internal Methods
        private void ValidateArguments()
        {
            if (string.IsNullOrEmpty(OutputPath))
                throw new ArgumentNullException(nameof(OutputPath));
            if (_sources.Count < 1 || _sources.Count > 2)
                throw new ArgumentException("Can only merge one or two source streams.");
        }
        #endregion

        #region Build Methods
        private Dictionary<MediaStreamType, MediaDecoder> MakeDecoders()
        {
            var decoders = new Dictionary<MediaStreamType, MediaDecoder>();
            foreach (var source in _sources.Values)
            {
                var decoder = new MediaDecoder(source.Path, HardwareDevice);
                switch (source.StreamType)
                {
                    case MediaStreamType.Audio:
                        decoder.SelectStream(AVMediaType.AVMEDIA_TYPE_AUDIO);
                        break;

                    case MediaStreamType.Video:
                        decoder.SelectStream(AVMediaType.AVMEDIA_TYPE_VIDEO);
                        break;

                    default:
                        throw new NotSupportedException($"Media stream type of {source.StreamType} is not supported.");
                }
                decoders.Add(source.StreamType, decoder);
            }
            return decoders;
        }

        public void Build()
        {
            var streams = new AVStream*[2];
            var stream_dic = new Dictionary<MediaStreamType, int>();

            ValidateArguments();

            // create decoders
            var decoders = MakeDecoders();
            try
            {
                // open output iocontext
                using var output = new IOContext(OutputPath, ffmpeg.AVIO_FLAG_WRITE);
                // open muxer
                using var muxer = new MediaMuxer(output, OutputShortName, OutputMimeType);
                // add streams
                var index = 0;
                foreach (var decoderPair in decoders)
                {
                    var decoder = decoderPair.Value;
                    var targetCodec = decoder.CodecId;
                    var decoderCodec = decoder.CodecContext;

                    switch (decoderPair.Key)
                    {
                        case MediaStreamType.Audio:
                            if (TargetAudioCodec != null)
                                targetCodec = TargetAudioCodec.Value;
                            break;

                        case MediaStreamType.Video:
                            if (TargetVideoCodec != null)
                                targetCodec = TargetVideoCodec.Value;
                            break;
                    }

                    var encoder = ffmpeg.avcodec_find_encoder(targetCodec);
                    var outStream = muxer.AddStream(encoder);
                    var param = outStream->codecpar;
                    streams[index] = outStream;
                    stream_dic.Add(decoderPair.Key, index++);

                    if (decoder.CodecId == targetCodec)
                    {
                        // converting to the same codec
                        ffmpeg.avcodec_parameters_from_context(param, decoder.GetStream()->codec).ThrowOnError();
                    }
                    else
                    {
                        // converting to another codec
                        switch (decoderPair.Key)
                        {
                            case MediaStreamType.Audio:
                                param->codec_id = targetCodec;
                                param->codec_type = AVMediaType.AVMEDIA_TYPE_AUDIO;
                                param->sample_rate = decoderCodec->sample_rate;
                                param->channels = decoderCodec->channels;
                                param->channel_layout = decoderCodec->channel_layout;
                                outStream->time_base = decoderCodec->time_base;
                                break;

                            case MediaStreamType.Video:
                                throw new NotSupportedException();
                        }
                    }
                    outStream->codecpar->codec_tag = 0;
                }

                // write headers
                muxer.WriteHeader();

                // write packets
                long audio_dts = ffmpeg.AV_NOPTS_VALUE, video_dts = ffmpeg.AV_NOPTS_VALUE;
                long audio_pts = 0, video_pts = 0;
                var audio_stream = decoders.Where(pair => pair.Key == MediaStreamType.Audio).First();
                var video_stream = decoders.Where(pair => pair.Key == MediaStreamType.Video).First();
                bool any_audio = true, any_video = true;

                while (true)
                {
                    bool anyPacket = false;

                    while (any_audio || any_video)
                    {
                        KeyValuePair<MediaStreamType, MediaDecoder> decoderPair;

                        // choose a decoder pair
                        if (!any_audio)
                            decoderPair = video_stream;
                        else if (!any_video)
                            decoderPair = audio_stream;
                        else
                        {
                            // choose between audio and video
                            decoderPair = video_dts < audio_dts ? video_stream : audio_stream;
                        }

                        // decoder is chosen now,
                        // let's read and encode
                        var decoder = decoderPair.Value;
                        var stream_index = stream_dic[decoderPair.Key];
                        var outputStream = streams[stream_index];

                        if (decoder.CodecId == outputStream->codec->codec_id)
                        {
                            // simply copy to target stream
                            using var inputFrame = decoder.ReadPacket();
                            if (inputFrame == null)
                            {
                                if (decoder == audio_stream.Value)
                                    any_audio = false;
                                else
                                    any_video = false;
                                continue;
                            }
                            var pck = inputFrame.Pointer;
                            ffmpeg.av_packet_rescale_ts(pck, decoder.TimeBase, outputStream->time_base);
                            pck->stream_index = stream_index;

                            long* last_dts, last_pts;
                            switch (decoder.CodecContext->codec_type)
                            {
                                case AVMediaType.AVMEDIA_TYPE_AUDIO:
                                    last_dts = &audio_dts;
                                    last_pts = &audio_pts;
                                    break;

                                case AVMediaType.AVMEDIA_TYPE_VIDEO:
                                    last_dts = &video_dts;
                                    last_pts = &video_pts;
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }

                            if (pck->dts < (*last_dts + ((muxer.FormatContextPtr->oformat->flags & ffmpeg.AVFMT_TS_NONSTRICT) > 0 ? 0 : 1)) && pck->dts != ffmpeg.AV_NOPTS_VALUE && *last_dts != ffmpeg.AV_NOPTS_VALUE)
                            {
                                var next_dts = (*last_dts) + 1;
                                if (pck->pts >= pck->dts && pck->pts != ffmpeg.AV_NOPTS_VALUE)
                                    pck->pts = Math.Max(pck->pts, next_dts);

                                if (pck->pts == ffmpeg.AV_NOPTS_VALUE)
                                    pck->pts = next_dts;
                                pck->dts = next_dts;
                            }
                                (*last_dts) = pck->dts;

                            muxer.WritePacket(pck);
                            anyPacket = true;
                        }
                        else
                            throw new NotSupportedException("Format conversion is not supported.");
                    }

                    if (!anyPacket)
                        break;
                }

                // write trailer
                muxer.WriteTrailer();
            }
            finally
            {
                // dispose decoders
                foreach (var decoder in decoders.Values)
                    decoder.Dispose();
            }
        }
        #endregion
    }
}
