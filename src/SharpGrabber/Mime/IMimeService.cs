namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Service used by grabbers to work with MIME types.
    /// </summary>
    public interface IMimeService
    {
        /// <summary>
        /// Extracts the file extension associated with <paramref name="mime"/>.
        /// </summary>
        string ExtractMimeExtension(string mime);
    }
}
