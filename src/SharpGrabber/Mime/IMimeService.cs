using System;

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

        /// <summary>
        /// Tries to find a mime type that represents the specified <paramref name="extension"/>.
        /// </summary>
        /// <returns>Whether or not the extension could be matched with a mime type.</returns>
        bool TryGetMimeByExtension(string extension, out string mime);
    }

    public static class MimeServiceExtensions
    {
        /// <summary>
        /// Tries to find a mime type that represents the specified <paramref name="extension"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the extension could not be matched with a mime type.
        /// </exception>
        public static string GetMimeByExtension(this IMimeService mimeService, string extension)
        {
            if (mimeService.TryGetMimeByExtension(extension, out var mime))
                return mime;
            throw new InvalidOperationException("The extension could not be matched with a mime type");
        }
    }
}
