using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Internal;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Grabs from various sources using grabbers registered on it.
    /// </summary>
    internal class MultiGrabber : MultiGrabberBase
    {
        public MultiGrabber(IGrabberServices services) : base(services)
        {
        }

        public MultiGrabber(IEnumerable<IGrabber> grabbers, IGrabberServices services) : base(grabbers, services)
        {
        }

        protected override async Task<GrabResult> InternalGrabAsync(IEnumerable<IGrabber> candidateGrabbers, Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            Exception lastException = null;

            foreach (var grabber in candidateGrabbers)
            {
                try
                {
                    var result = await grabber.GrabAsync(uri, cancellationToken, options, progress).ConfigureAwait(false);
                    if (result != null)
                        return result;
                }
                catch (Exception exception)
                {
                    lastException = exception;
                }
            }

            if (lastException == null)
                throw new UnsupportedGrabException("The URL is not supported.");

            throw lastException;

        }
    }
}
