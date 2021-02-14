using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Simple abstract implementation for <see cref="IGrabber"/>.
    /// </summary>
    public abstract class BaseGrabber : IGrabber
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public GrabOptions DefaultGrabOptions { get; } 
            = new GrabOptions(GrabOptionFlag.Decipher | GrabOptionFlag.GrabImages | GrabOptionFlag.Decrypt);

        /// <inheritdoc />
        public WorkStatus Status { get; } = new WorkStatus();

        /// <inheritdoc />
        public abstract bool Supports(Uri uri);

        /// <inheritdoc />
        public Task<GrabResult> GrabAsync(Uri uri) => GrabAsync(uri, new CancellationToken());

        /// <inheritdoc />
        public Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken)
        {
            Status.Update(null, "Initializing...", WorkStatusType.Initiating);
            return GrabAsync(uri, cancellationToken, DefaultGrabOptions);
        }

        /// <inheritdoc />
        public abstract Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options);
    }
}