using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Configures and builds a <see cref="IMultiGrabber"/>.
    /// </summary>
    public class GrabberBuilder : IGrabberBuilder
    {
        private readonly Dictionary<Type, IGrabber> _grabbers = new();
        private IGrabberServices _services;

        private GrabberBuilder()
        {
        }

        /// <summary>
        /// Creates a new builder.
        /// </summary>
        public static IGrabberBuilder New()
            => new GrabberBuilder().UseDefaultServices();

        public IGrabberBuilder Add(IGrabber grabber)
        {
            var t = grabber.GetType();
            if (_grabbers.ContainsKey(t))
                throw new InvalidOperationException($"The grabber is already included: {t}");
            _grabbers.Add(t, grabber);
            return this;
        }

        public IGrabberBuilder Add<T>() where T : IGrabber
        {
            var grabber = (IGrabber)Activator.CreateInstance(typeof(T), _services);
            return Add(grabber);
        }

        public IGrabberBuilder UseServices(IGrabberServices services)
        {
            _services = services;
            return this;
        }

        public IMultiGrabber Build()
        {
            if (_services == null)
                throw new InvalidOperationException($"An instance of {nameof(IGrabberServices)} must be set.");

            return new MultiGrabber(_grabbers.Values, _services);
        }
    }
}
