using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetTools.SharpGrabber.Converter
{
    public unsafe sealed class MediaConcatenator
    {
        private readonly List<Uri> _sources = new List<Uri>();

        public MediaConcatenator() { }

        public MediaConcatenator(string outputPath)
        {
            OutputPath = outputPath;
        }

        /// <summary>
        /// Path to the output file
        /// </summary>
        public string OutputPath { get; set; }

        public string OutputExtension { get; set; } = "mp4";

        public string OutputMimeType { get; set; } = "video/mp4";

        public AVHWDeviceType HardwareDevice { get; set; }

        public void AddSource(Uri path)
        {
            _sources.Add(path);
        }

        public void AddSource(string path) => AddSource(new Uri($"file://{path.Replace('\\', '/')}"));

        public void AutoSelectHardwareDevice()
        {
            var types = MediaHelper.GetHardwareDeviceTypes();
            if (types.Length > 0)
                HardwareDevice = types[0];
        }

        public void Build()
        {
            var streams = new AVStream*[2];
            var stream_dic = new Dictionary<MediaStreamType, int>();

            ValidateArguments();

            // open output iocontext
            using var output = new IOContext(OutputPath, ffmpeg.AVIO_FLAG_WRITE);
            // open muxer
            using var muxer = new MediaMuxer(output, OutputExtension, OutputMimeType);

            // create output streams
            var firstSource = _sources.First();
            AddOutputStreams(firstSource, muxer, out var vidOutStream, out var audioOutStream, out var streamMap);

            // write headers
            muxer.WriteHeader();

            // concat media files
            var context = new ConcatContext
            {
                Output = output,
                Muxer = muxer,
                VideoOutStream = vidOutStream,
                AudioOutStream = audioOutStream,
                StreamMap = streamMap,
            };
            foreach (var source in _sources)
            {
                ConcatFile(source, context);
            }

            // write trailer
            muxer.WriteTrailer();
        }

        private class ConcatContext
        {
            public IOContext Output { get; set; }
            public MediaMuxer Muxer { get; set; }
            public AVStream* VideoOutStream { get; set; }
            public AVStream* AudioOutStream { get; set; }
            public IDictionary<MediaStreamType, int> StreamMap { get; set; }
        }

        private void ConcatFile(Uri source, ConcatContext context)
        {
            using var videoDecoder = new MediaDecoder(source, HardwareDevice);
            using var audioDecoder = new MediaDecoder(source, HardwareDevice);
            var hasVideo = new Reference<bool>(videoDecoder.SelectStream(AVMediaType.AVMEDIA_TYPE_VIDEO));
            var hasAudio = new Reference<bool>(audioDecoder.SelectStream(AVMediaType.AVMEDIA_TYPE_AUDIO));
            var audioDts = new Reference<long>();
            var videoDts = new Reference<long>();

            while (hasVideo || hasAudio)
            {
                MediaStreamType type;
                if (!hasAudio)
                    type = MediaStreamType.Video;
                else if (!hasVideo)
                    type = MediaStreamType.Audio;
                else
                    type = audioDts.Value < videoDts.Value ? MediaStreamType.Audio : MediaStreamType.Video;

                AVStream* outStream;
                MediaDecoder decoder;
                Reference<long> dts;
                Reference<bool> has;
                switch (type)
                {
                    case MediaStreamType.Audio:
                        outStream = context.AudioOutStream;
                        decoder = audioDecoder;
                        dts = audioDts;
                        has = hasAudio;
                        break;
                    case MediaStreamType.Video:
                        outStream = context.VideoOutStream;
                        decoder = videoDecoder;
                        dts = videoDts;
                        has = hasVideo;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                using var packet = decoder.ReadPacket();
                if (packet == null)
                {
                    has.Value = false;
                    continue;
                }
                var pck = packet.Pointer;
                ffmpeg.av_packet_rescale_ts(pck, decoder.TimeBase, outStream->time_base);
                pck->stream_index = context.StreamMap[type];
                dts.Value = pck->dts;

                context.Muxer.WritePacket(pck);
            }
        }

        private void AddOutputStreams(Uri source, MediaMuxer muxer, out AVStream* vidOutStream,
            out AVStream* audioOutStream, out IDictionary<MediaStreamType, int> streamMap)
        {
            vidOutStream = audioOutStream = null;
            using var audioDecoder = new MediaDecoder(source, HardwareDevice);
            using var videoDecoder = new MediaDecoder(source, HardwareDevice);
            var hasAudio = audioDecoder.SelectStream(AVMediaType.AVMEDIA_TYPE_AUDIO);
            var hasVideo = videoDecoder.SelectStream(AVMediaType.AVMEDIA_TYPE_VIDEO);
            var streamIndex = 0;
            streamMap = new Dictionary<MediaStreamType, int>();

            var decoders = new List<MediaDecoder>();
            if (hasVideo)
                decoders.Add(videoDecoder);
            if (hasAudio)
                decoders.Add(audioDecoder);
            foreach (var decoder in decoders)
            {
                var targetCodec = decoder.CodecId;
                var decoderCodec = decoder.CodecContext;

                AVStream* stream;
                var encoder = ffmpeg.avcodec_find_encoder(targetCodec);

                stream = muxer.AddStream(encoder);
                if (decoder == videoDecoder)
                {
                    vidOutStream = stream;
                    streamMap.Add(MediaStreamType.Video, streamIndex);
                }
                else if (decoder == audioDecoder)
                {
                    audioOutStream = stream;
                    streamMap.Add(MediaStreamType.Audio, streamIndex);
                }
                else
                    throw new NotImplementedException("Unknown decoder type.");
                streamIndex++;
                //streams[streamIndex] = stream;
                var param = stream->codecpar;
                ffmpeg.avcodec_parameters_from_context(param, decoder.CodecContext).ThrowOnError();
                stream->codecpar->codec_tag = 0;
            }
        }

        private void ValidateArguments()
        {
            if (string.IsNullOrEmpty(OutputPath))
                throw new ArgumentNullException(nameof(OutputPath));
        }
    }
}
