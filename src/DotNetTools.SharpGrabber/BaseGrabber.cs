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
        public abstract string Name { get; }

        public GrabOptions DefaultGrabOptions { get; } = new GrabOptions(GrabOptionFlag.Decipher);

        public WorkStatus Status { get; } = new WorkStatus();

        public abstract bool Supports(Uri uri);

        public Task<GrabResult> Grab(Uri uri) => Grab(uri, new CancellationToken());

        public Task<GrabResult> Grab(Uri uri, CancellationToken cancellationToken)
        {
            Status.Update(null, "Initializing...", WorkStatusType.Initiating);
            return Grab(uri, cancellationToken, DefaultGrabOptions);
        }

        public abstract Task<GrabResult> Grab(Uri uri, CancellationToken cancellationToken, GrabOptions options);
    }
}