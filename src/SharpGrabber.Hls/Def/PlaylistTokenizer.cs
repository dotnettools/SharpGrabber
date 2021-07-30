using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Hls
{
    /// <summary>
    /// Tokenizer for M3U8 files
    /// </summary>
    public class PlaylistTokenizer : IDisposable
    {
        private const string M3UHeader = "#EXTM3U";
        private const string TagMark = "#EXT";

        private readonly object _internalLock = new object();
        private readonly StreamReader _reader;

        public PlaylistTokenizer(StreamReader reader)
        {
            _reader = reader;
        }

        public PlaylistTokenizer(Stream stream, bool ownStream = true)
            : this(new StreamReader(stream, Encoding.UTF8, false, 1024, !ownStream))
        {
        }

        #region Properties
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Whether or not the stream has reached its end
        /// </summary>
        public bool EndOfStream => _reader.EndOfStream;

        /// <summary>
        /// The <see cref="StringComparison"/> used by the tokenizer
        /// </summary>
        public StringComparison Comparison { get; set; } = StringComparison.InvariantCulture;
        #endregion

        #region Methods
        /// <summary>
        /// Tries to read the next token from the input stream.
        /// </summary>
        /// <returns>The read token, or NULL if the input stream has reached its end.</returns>
        public PlaylistToken Read()
        {
            while (!_reader.EndOfStream)
            {
                var content = _reader.ReadLine();
                var token = ParseToken(content);
                if (token != null)
                    return token;
            }

            return null;
        }

        /// <summary>
        /// Tries to read the next token from the input stream asynchronously.
        /// </summary>
        /// <returns>The read token, or NULL if the input stream has reached its end.</returns>
        public async Task<PlaylistToken> ReadAsync()
        {
            while (!_reader.EndOfStream)
            {
                var content = await _reader.ReadLineAsync().ConfigureAwait(false);
                var token = ParseToken(content);
                if (token != null)
                    return token;
            }

            return null;
        }

        public void Dispose()
        {
            lock (_internalLock)
            {
                if (IsDisposed)
                    return;
                IsDisposed = true;
            }

            _reader.Dispose();
        }
        #endregion

        #region Internal Methods
        private PlaylistToken ParseToken(string content)
        {
            // Test blank line
            content = content?.Trim();
            if (string.IsNullOrEmpty(content))
                return null;

            // Test M3U header
            if (M3UHeader.Equals(content, Comparison))
                return new PlaylistToken(PlaylistTokenType.Header);

            // Test if the line starts with sharp (#)
            if (content[0] == '#')
            {
                // Test if the line starts with the tag mark
                if (content.StartsWith(TagMark, Comparison))
                    return new PlaylistToken(PlaylistTokenType.Tag, content);

                // Comment detected
                return new PlaylistToken(PlaylistTokenType.Comment, content);
            }
            else
            {
                // Uri line detected
                return new PlaylistToken(PlaylistTokenType.Uri, content);
            }
        }
        #endregion
    }
}