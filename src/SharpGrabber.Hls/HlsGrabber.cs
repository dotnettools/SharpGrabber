using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Hls.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Hls
{
    public class HlsGrabber : GrabberBase
    {
        /// <summary>
        /// Gets the initializer for the assembly.
        /// </summary>
        public static Type Initializer => typeof(AssemblyInitializer);

        private const string MpegUrlContentType = "application/x-mpegurl";
        private const string M3u8Extension = ".m3u8";
        private static readonly MediaFormat PlaylistFormat = new MediaFormat("application/x-mpegURL", "m3u8");
        private static readonly MediaFormat OutputFormat = new MediaFormat("video/MP2T", "ts");

        public HlsGrabber(IGrabberServices services) : base(services)
        {
        }

        public override string StringId { get; } = "hls";

        public override string Name { get; } = "M3U8";

        public override bool Supports(Uri uri)
        {
            return uri.AbsolutePath.EndsWith(M3u8Extension, StringComparison.InvariantCultureIgnoreCase);
        }

        protected override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress)
        {
            var client = Services.GetClient();
            using var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var doc = new PlaylistDocument();
            try
            {
                await doc.LoadAsync(responseStream).ConfigureAwait(false);
            }
            catch (PlaylistDocumentLoadException loadException)
            {
                throw new GrabException("Failed to load the M3U8 playlist.", loadException);
            }
            return await GrabAsync(uri, doc, cancellationToken, options).ConfigureAwait(false);
        }

        private Task<GrabResult> GrabAsync(Uri uri, PlaylistDocument doc, CancellationToken cancellationToken,
            GrabOptions options)
        {
            // get resources
            IList<IGrabbed> grabs;
            if (doc.Streams.Count > 0)
                grabs = GrabStreams(uri, doc);
            else if (doc.Segments.Count > 0)
                grabs = GrabSegments(uri, doc);
            else
                grabs = null;

            // make result
            var result = new GrabResult(uri, new ListWrapper<IGrabbed>(grabs));
            if (options.Flags.HasFlag(GrabOptionFlags.Decrypt) && doc.Key != null)
                UpdateDecryptionMethod(result, doc.Key);
            return Task.FromResult(result);
        }

        private void UpdateDecryptionMethod(GrabResult result, HlsKey key)
        {
            switch (key.Method)
            {
                case HlsKeyMethod.Aes128:
                    var decryptor = new HlsAes128Decryptor(key, Services);
                    result.OutputStreamWrapper = decryptor.WrapStreamAsync;
                    break;

                case HlsKeyMethod.None:
                    break;

                default:
                    throw new NotSupportedException($"HLS grab error: Decrypting {key.Method} is not supported.");
            }
        }

        private IList<IGrabbed> GrabStreams(Uri originalUri, PlaylistDocument doc)
        {
            var list = new List<GrabbedHlsStreamMetadata>();
            foreach (var stream in doc.Streams)
            {
                var uri = new Uri(originalUri, stream.Uri);
                var resolvableStream = new ResolvableStream(uri, stream, Services);
                var g = new GrabbedHlsStreamMetadata( uri, stream.Name,
                    stream.Resolution, stream.Bandwidth, PlaylistFormat, OutputFormat, new Lazy<Task<GrabbedHlsStream>>(resolvableStream.Resolve));
                list.Add(g);
            }
            return list.OrderByDescending(s => s.Resolution.Height).ToList<IGrabbed>();
        }

        private IList<IGrabbed> GrabSegments(Uri originalUri, PlaylistDocument doc)
        {
            var list = new List<IGrabbed>();
            var segments = new List<MediaSegment>();
            var totalDuration = TimeSpan.Zero;

            foreach (var segment in doc.Segments)
            {
                totalDuration += segment.Duration;
                var uri = new Uri(originalUri, segment.Uri);
                segments.Add(new MediaSegment(segment.Title, uri, segment.Duration));
            }

            var g = new GrabbedHlsStream
            {
                OriginalUri = originalUri,
                ResourceUri = originalUri,
                Length = totalDuration,
                Segments = segments,
            };
            var resolvableStream = new ResolvableStream(g);
            list.Add(new GrabbedHlsStreamMetadata(originalUri, "Media", null, 0, PlaylistFormat, OutputFormat,
                new Lazy<Task<GrabbedHlsStream>>(resolvableStream.Resolve)));
            return list;
        }

        private class ResolvableStream
        {
            private readonly Uri _uri;
            private readonly HlsStreamInfo _stream;
            private readonly GrabbedHlsStream _resolved;
            private readonly IGrabberServices _services;

            public ResolvableStream(Uri uri, HlsStreamInfo stream, IGrabberServices services)
            {
                _uri = uri;
                _stream = stream;
                _services = services;
            }

            public ResolvableStream(GrabbedHlsStream resolved)
            {
                _resolved = resolved;
            }

            public async Task<GrabbedHlsStream> Resolve()
            {
                if (_resolved != null)
                    return _resolved;

                var grabber = new HlsGrabber(_services);
                var result = await grabber.GrabAsync(_uri).ConfigureAwait(false);
                return await result.Resources.OfType<GrabbedHlsStreamMetadata>().Single().Stream.Value.ConfigureAwait(false);
            }
        }
    }
}
