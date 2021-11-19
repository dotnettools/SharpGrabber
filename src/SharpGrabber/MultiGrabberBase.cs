using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace DotNetTools.SharpGrabber
{
    public abstract class MultiGrabberBase : IMultiGrabber
    {
        private readonly ConcurrentHashSet<IGrabber> _grabbers;

        public MultiGrabberBase(IEnumerable<IGrabber> grabbers, IGrabberServices services)
        {
            Services = services;
            grabbers ??= Array.Empty<IGrabber>();
            _grabbers = new ConcurrentHashSet<IGrabber>(grabbers);
            GrabbersUpdated();
        }

        public MultiGrabberBase(IGrabberServices services) : this(null, services)
        {

        }

        public string StringId { get; } = null;

        public string Name { get; private set; }

        public GrabOptions DefaultGrabOptions => new(GrabOptionFlags.All);

        public IGrabberServices Services { get; }

        public bool Supports(Uri uri)
            => _grabbers.Any(g => g.Supports(uri));

        public Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            progress ??= new Progress<double>();
            var candidateGrabbers = _grabbers.Where(g => g.Supports(uri));

            return InternalGrabAsync(candidateGrabbers, uri, cancellationToken, options, progress);
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

        protected abstract Task<GrabResult> InternalGrabAsync(IEnumerable<IGrabber> candidateGrabbers, Uri uri, CancellationToken cancellationToken,
            GrabOptions options, IProgress<double> progress);

        private void GrabbersUpdated()
        {
            Name = string.Join(", ", _grabbers.Select(g => g.Name)).NullIfEmpty();
        }
    }
}
