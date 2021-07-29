using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Default implementation for <see cref="IGrabResult"/>
    /// </summary>
    public class GrabResult : IGrabResult
    {
        private StreamWrappingDelegate _streamWrappingDelegate = stream => Task.FromResult(stream);

        public GrabResult(Uri originalUri)
        {
            Resources = new List<IGrabbed>();
            OriginalUri = originalUri;
        }

        public GrabResult(Uri originalUri, IReadOnlyList<IGrabbed> grabbedList)
        {
            Resources = grabbedList;
            OriginalUri = originalUri;
        }

        public Uri OriginalUri { get; }

        public IReadOnlyList<IGrabbed> Resources { get; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? CreationDate { get; set; }

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

        public Task<Stream> WrapStreamAsync(Stream stream)
            => OutputStreamWrapper.Invoke(stream);
    }
}