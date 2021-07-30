using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Returned as the result of a grab operation.
    /// Contains resources and kind of information scraped from the target URI.
    /// </summary>
    public interface IGrabResult
    {
        /// <summary>
        /// Gets the original grab request URI.
        /// </summary>
        public Uri OriginalUri { get; }

        /// <summary>
        /// Gets the list of grabbed resources.
        /// </summary>
        public IReadOnlyList<IGrabbed> Resources { get; }

        /// <summary>
        /// Gets the subject of the page.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the description of the content.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the optional date/time when this content was created.
        /// </summary>
        public DateTime? CreationDate { get; }

        /// <summary>
        /// Gets whether the media is secured. This has different meanings for different grabbers.
        /// For instance, <see cref="IsSecure"/> is true if a YouTube video has signature and its 
        /// download links need to be deciphered.
        /// </summary>
        public bool IsSecure { get; }

        /// <summary>
        /// Wraps a stream with another one that reads from the original stream and processes the data when necessary;
        /// giving the grabber a chance to decrypt the incoming data.
        /// This method should always be invoked after downloading a media file or segment.
        /// </summary>
        /// <remarks>
        /// The original and the result stream must be disposed by the caller.
        /// </remarks>
        Task<Stream> WrapStreamAsync(Stream stream);
    }
}
