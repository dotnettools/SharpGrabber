namespace DotNetTools.SharpGrabber.Hls
{
    /// <summary>
    /// Describes type of a <see cref="PlaylistToken"/> read by the <see cref="PlaylistTokenizer"/>.
    /// </summary>
    public enum PlaylistTokenType
    {
        /// <summary>
        /// Indicates the EXTM3U Header.
        /// </summary>
        Header,

        /// <summary>
        /// Indicates a URI line.
        /// </summary>
        Uri,

        /// <summary>
        /// Indicates a comment line.
        /// </summary>
        Comment,

        /// <summary>
        /// Indicates a tag line.
        /// </summary>
        Tag,
    }
}