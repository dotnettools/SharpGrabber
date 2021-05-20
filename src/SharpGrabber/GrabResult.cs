using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Result of a grab request - <see cref="GrabResult"/> is returned by grabbers containing grabbed resources and
    /// information obtained from the page.
    /// </summary>
    public class GrabResult
    {
        private StreamWrappingDelegate _streamWrappingDelegate = stream => Task.FromResult(stream);

        public GrabResult(Uri originalUri)
        {
            Resources = new List<IGrabbed>();
            OriginalUri = originalUri;
        }

        public GrabResult(Uri originalUri, IList<IGrabbed> grabbedList)
        {
            Resources = grabbedList;
            OriginalUri = originalUri;
        }

        /// <inheritdoc />
        public Uri OriginalUri { get; }

        /// <summary>
        /// List of grabbed resources
        /// </summary>
        public IList<IGrabbed> Resources { get; }

        /// <summary>
        /// Subject of the page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the content
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional date and time when this content was created
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Optional statistics grabbed from the page
        /// </summary>
        public GrabStatisticInfo Statistics { get; set; }

        /// <summary>
        /// Whether or not the media is secured. For example, <see cref="IsSecure"/> is true if a YouTube video
        /// has signature and its download links need to be deciphered.
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// Gets or sets the function that accepts an stream and wraps it with other streams if necessary.
        /// This value cannot be NULL.
        /// </summary>
        public StreamWrappingDelegate OutputStreamWrapper
        {
            get => _streamWrappingDelegate;
            set => _streamWrappingDelegate = value ?? throw new ArgumentNullException(nameof(OutputStreamWrapper));
        }

        /// <summary>
        /// Accepts an stream and wraps it with other streams if necessary.
        /// This method should always be invoked after downloading a media file or segment.
        /// </summary>
        /// <remarks>
        /// The original and the result stream should be disposed by the caller.
        /// </remarks>
        public Task<Stream> WrapStreamAsync(Stream stream)
            => OutputStreamWrapper.Invoke(stream);
    }
}