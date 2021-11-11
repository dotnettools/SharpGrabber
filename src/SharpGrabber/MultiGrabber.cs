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
    internal class MultiGrabber : IMultiGrabber
    {
        private readonly ConcurrentHashSet<IGrabber> _grabbers;

        public MultiGrabber(IEnumerable<IGrabber> grabbers, IGrabberServices services)
        {
            Services = services;
            grabbers ??= Array.Empty<IGrabber>();
            _grabbers = new ConcurrentHashSet<IGrabber>(grabbers);
            GrabbersUpdated();
        }

        public MultiGrabber(IGrabberServices services) : this(null, services)
        {

        }

        public string StringId { get; } = null;

        public string Name { get; private set; }

        public GrabOptions DefaultGrabOptions => null;

        public IGrabberServices Services { get; }

        public bool Supports(Uri uri)
            => _grabbers.Any(g => g.Supports(uri));

        public async Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            progress ??= new Progress<double>();
            Exception lastException = null;
            var candidateGrabbers = _grabbers.Where(g => g.Supports(uri));

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

        public IEnumerable<IGrabber> GetRegisteredGrabbers()
            => _grabbers.AsEnumerable();

        public void Register(IGrabber grabber)
        {
            _grabbers.Add(grabber);
            GrabbersUpdated();
        }

        public void Unregister(IGrabber grabber)
        {
            _grabbers.Remove(grabber);
            GrabbersUpdated();
        }

        private void GrabbersUpdated()
        {
            Name = string.Join(", ", _grabbers.Select(g => g.Name)).NullIfEmpty();
        }
    }
}
