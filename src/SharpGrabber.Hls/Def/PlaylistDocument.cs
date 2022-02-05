using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Hls
{
    /// <summary>
    /// Works with M3U8 files.
    /// </summary>
    public class PlaylistDocument
    {
        private static readonly Regex _resolutionRegex = new Regex(@"^(\d+)\D+(\d+)$", RegexOptions.Compiled);

        private readonly Dictionary<string, Action<PlaylistTagValue, LoadContext>> _tagParsers
            = new Dictionary<string, Action<PlaylistTagValue, LoadContext>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly List<HlsMediaSegment> _segments = new List<HlsMediaSegment>();
        private readonly List<HlsStreamInfo> _streams = new List<HlsStreamInfo>();

        public PlaylistDocument()
        {
            Segments = _segments.AsReadOnly();
            Streams = _streams.AsReadOnly();
            _tagParsers.Add("EXT-X-VERSION", ParseExtVersion);
            _tagParsers.Add("EXT-X-TARGETDURATION", ParseExtTargetDuration);
            _tagParsers.Add("EXT-X-MEDIA-SEQUENCE", ParseExtMediaSequence);
            _tagParsers.Add("EXT-X-KEY", ParseExtKey);
            _tagParsers.Add("EXTINF", ParseExtInf);
            _tagParsers.Add("EXT-X-STREAM-INF", ParseStreamInf);
        }

        public int? DocumentVersion { get; private set; }
        public TimeSpan? TargetDuration { get; private set; }
        public int MediaSequence { get; private set; }
        public HlsKey Key { get; private set; }
        public IReadOnlyList<HlsMediaSegment> Segments { get; }
        public IReadOnlyList<HlsStreamInfo> Streams { get; }

        /// <summary>
        /// Clears data of this instance.
        /// </summary>
        public void Clear()
        {
            DocumentVersion = null;
            TargetDuration = null;
            MediaSequence = 0;
            Key = null;
            _segments.Clear();
        }

        /// <summary>
        /// Loads the playlist from a stream.
        /// </summary>
        /// <exception cref="PlaylistDocumentLoadException" />
        public Task LoadAsync(StreamReader reader, Uri originalUri)
        {
            using var tokenizer = new PlaylistTokenizer(reader);
            return LoadAsync(tokenizer, originalUri);
        }

        /// <summary>
        /// Loads the playlist from a stream.
        /// </summary>
        /// <exception cref="PlaylistDocumentLoadException" />
        public Task LoadAsync(Stream stream, Uri baseUri)
        {
            using var reader = new StreamReader(stream);
            return LoadAsync(reader, baseUri);
        }

        /// <summary>
        /// Loads the playlist from a string.
        /// </summary>
        /// <exception cref="PlaylistDocumentLoadException" />
        public async Task LoadAsync(string content, Uri baseUri)
        {
            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            streamWriter.Write(content);
            streamWriter.Flush();
            stream.Position = 0;
            await LoadAsync(stream, baseUri).ConfigureAwait(false);
        }

        private class LoadContext
        {
            public Uri BaseUri { get; set; }
            public PlaylistToken Token { get; set; }
            public PlaylistTagValue TagValue { get; set; }
            public PlaylistTagValue ExtInf { get; set; }
            public PlaylistTagValue StreamInf { get; set; }
        }

        private async Task LoadAsync(PlaylistTokenizer tokenizer, Uri originalUri)
        {
            Clear();

            // Read header
            var token = await tokenizer.ReadAsync().ConfigureAwait(false);
            if (token == null || token.Type != PlaylistTokenType.Header)
                throw new PlaylistDocumentLoadException();

            var context = new LoadContext
            {
                BaseUri = originalUri,
            };
            while (!tokenizer.EndOfStream)
            {
                token = await tokenizer.ReadAsync().ConfigureAwait(false);
                if (token == null)
                    break;
                context.Token = token;

                switch (token.Type)
                {
                    case PlaylistTokenType.Comment:
                        continue;

                    case PlaylistTokenType.Header:
                        throw new PlaylistDocumentLoadException("Unexpected HEADER.");

                    case PlaylistTokenType.Tag:
                        ParseTagToken(context);
                        break;

                    case PlaylistTokenType.Uri:
                        ParseUriToken(context);
                        break;

                    default:
                        throw new NotImplementedException($"Playlist token type of '{token.Type}' is not implemented.");
                }
            }
        }

        private void ParseTagToken(LoadContext context)
        {
            if (context.ExtInf != null || context.StreamInf != null)
                throw new PlaylistDocumentLoadException("Expected URI, but got TAG.");

            var content = context.Token.Content;
            if (content[0] == '#')
                content = content.Substring(1);
            var tagValue = context.TagValue = PlaylistTagValue.Parse(content);
            var parser = _tagParsers.GetOrDefault(tagValue.Key);
            if (parser != null)
                parser.Invoke(tagValue, context);
        }

        private void ParseUriToken(LoadContext context)
        {
            var uri = new Uri(context.Token.Content, UriKind.RelativeOrAbsolute);
            if (context.ExtInf != null)
            {
                var tag = context.ExtInf;
                var data = tag.WholeValue.Split(',');
                var duration = TimeSpan.FromSeconds(double.Parse(data[0]));
                var title = data.Length > 1 ? data[1] : null;
                if (string.IsNullOrEmpty(title))
                    title = null;
                _segments.Add(new HlsMediaSegment(uri, duration, title));
            }
            else if (context.StreamInf != null)
            {
                var tag = context.StreamInf;
                var sProgramId = tag.Values.GetOrDefault("PRORGAM-ID");
                var sBandwidth = tag.Values.GetOrDefault("BANDWIDTH");
                var sResolution = tag.Values.GetOrDefault("RESOLUTION");
                var name = tag.Values.GetOrDefault("NAME");
                var programId = string.IsNullOrEmpty(sProgramId) ? 0 : int.Parse(sProgramId);
                var bandwidth = string.IsNullOrEmpty(sBandwidth) ? 0 : int.Parse(sBandwidth);
                RectSize resolution = null;
                if (!string.IsNullOrEmpty(sResolution))
                {
                    var match = _resolutionRegex.Match(sResolution);
                    if (match.Success)
                        resolution = new RectSize(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                }
                _streams.Add(new HlsStreamInfo(uri, programId, bandwidth, resolution, name));
            }
            else
                throw new PlaylistDocumentLoadException("Expected info TAG, but got URI.");

            context.ExtInf = context.StreamInf = null;
        }

        private void ParseExtInf(PlaylistTagValue tag, LoadContext context)
        {
            context.ExtInf = tag;
        }

        private void ParseStreamInf(PlaylistTagValue tag, LoadContext context)
        {
            context.StreamInf = tag;
        }

        private void ParseExtVersion(PlaylistTagValue tag, LoadContext context)
        {
            DocumentVersion = int.Parse(tag.WholeValue);
        }

        private void ParseExtMediaSequence(PlaylistTagValue tag, LoadContext context)
        {
            MediaSequence = int.Parse(tag.WholeValue);
        }

        private void ParseExtTargetDuration(PlaylistTagValue tag, LoadContext context)
        {
            TargetDuration = TimeSpan.FromSeconds(double.Parse(tag.WholeValue));
        }

        private void ParseExtKey(PlaylistTagValue tag, LoadContext context)
        {
            var sMethod = tag.Values.GetOrDefault("METHOD");
            var sUri = tag.Values.GetOrDefault("URI");
            var sIV = tag.Values.GetOrDefault("IV");
            var method = (sMethod?.ToUpperInvariant()) switch
            {
                "AES-128" => HlsKeyMethod.Aes128,
                "SAMPLE-AES" => HlsKeyMethod.SampleAes,
                "NONE" => HlsKeyMethod.None,
                _ => throw new NotSupportedException($"HLS encryption method '{sMethod}' is not supported."),
            };
            Uri uri = null;
            if (!string.IsNullOrEmpty(sUri) && !Uri.TryCreate(sUri, UriKind.RelativeOrAbsolute, out uri))
                throw new PlaylistDocumentLoadException($"Invalid URI format: {sUri}");
            if (!uri.IsAbsoluteUri)
                uri = new Uri(context.BaseUri, uri);

            if (!string.IsNullOrEmpty(sIV) && sIV.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                sIV = sIV.Substring(2);
            var iv = string.IsNullOrEmpty(sIV) ? null : StringToByteArray(sIV);

            Key = new HlsKey(method, uri, iv);
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
