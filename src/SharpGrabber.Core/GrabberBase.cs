using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Base implementation for <see cref="IGrabber"/>
    /// </summary>
    public abstract class GrabberBase : IGrabber
    {
        protected GrabberBase(IGrabberServices services)
        {
            Services = services;
        }

        public abstract string Name { get; }

        public virtual GrabOptions DefaultGrabOptions { get; } = new GrabOptions(GrabOptionFlags.None);

        public IGrabberServices Services { get; }

        public abstract bool Supports(Uri uri);

        public async Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            progress.Report(0);
            options ??= DefaultGrabOptions;

            var result = await GrabAsync(uri,
                cancellationToken,
                options,
                progress ?? new Progress<double>()).ConfigureAwait(false);

            progress.Report(1);
            return result;
        }

        /// <summary>
        /// The same as <see cref="GrabAsync(Uri, CancellationToken, GrabOptions, IProgress{double})"/>, except all parameters are necessarily supplied for convenience.
        /// </summary>
        protected abstract Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress);
    }
}
