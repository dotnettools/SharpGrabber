using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Hls;
using DotNetTools.SharpGrabber.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.Hls
{
    public class HlsGrabber : BaseGrabber
    {
        private const string MpegUrlContentType = "application/x-mpegurl";
        private const string M3u8Extension = ".m3u8";
        private static readonly MediaFormat PlaylistFormat = new MediaFormat("application/x-mpegURL", "m3u8");
        private static readonly MediaFormat OutputFormat = new MediaFormat("video/MP2T", "ts");

        public override string Name { get; } = "M3U8";

        public override bool Supports(Uri uri)
        {
            return uri.AbsolutePath.EndsWith(M3u8Extension, StringComparison.InvariantCultureIgnoreCase);
        }

        public override async Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options)
        {
            var client = HttpHelper.GetClient(uri);
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
            var result = new GrabResult(uri, grabs);
            if (options.Flags.HasFlag(GrabOptionsFlags.Decrypt) && doc.Key != null)
                UpdateDecryptionMethod(result, doc.Key);
            return Task.FromResult(result);
        }

        private void UpdateDecryptionMethod(GrabResult result, HlsKey key)
        {
            switch (key.Method)
            {
                case HlsKeyMethod.Aes128:
                    var decryptor = new HlsAes128Decryptor(key);
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
            var list = new List<GrabbedStreamMetadata>();
            foreach (var stream in doc.Streams)
            {
                var uri = new Uri(originalUri, stream.Uri);
                var g = new GrabbedStreamMetadata(originalUri, uri, stream.Name,
                    stream.Resolution, stream.Bandwidth, PlaylistFormat, OutputFormat, new ResolvableStream(uri, stream));
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

            var g = new GrabbedStream(originalUri, originalUri, totalDuration, segments);
            list.Add(new GrabbedStreamMetadata(originalUri, originalUri, "Media", null, 0, PlaylistFormat, OutputFormat,
                new ResolvableStream(g)));
            return list;
        }

        private class ResolvableStream : BaseResolvable<GrabbedStream>
        {
            private readonly Uri _uri;
            private readonly HlsStreamInfo _stream;
            private readonly GrabbedStream _resolved;

            public ResolvableStream(Uri uri, HlsStreamInfo stream)
            {
                _uri = uri;
                _stream = stream;
            }

            public ResolvableStream(GrabbedStream resolved)
            {
                _resolved = resolved;
            }

            protected override async Task<GrabbedStream> InternalResolve()
            {
                if (_resolved != null)
                    return _resolved;

                var grabber = new HlsGrabber();
                var result = await grabber.GrabAsync(_uri).ConfigureAwait(false);
                return await result.Resources.OfType<GrabbedStreamMetadata>().Single().Stream.ResolveAsync();
            }
        }
    }
}
