namespace DotNetTools.SharpGrabber.Hls
{
    /// <summary>
    /// Represents a token read from a M3U8 file by a <see cref="PlaylistTokenizer"/>.
    /// </summary>
    public sealed class PlaylistToken
    {
        public PlaylistToken(PlaylistTokenType type)
        {
            Type = type;
        }

        public PlaylistToken(PlaylistTokenType type, string content)
        {
            Type = type;
            Content = content;
        }

        public PlaylistTokenType Type { get; }
        public string Content { get; }

        public override string ToString()
            => $"{Type}('{Content}')";
    }
}