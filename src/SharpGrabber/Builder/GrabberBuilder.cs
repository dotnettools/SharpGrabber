using System;
using System.Collections.Generic;

namespace DotNetTools.SharpGrabber
{
    public class GrabberBuilder : IGrabberBuilder
    {
        private readonly HashSet<IGrabber> _grabbers = new();
        private IGrabberServices _services;

        private GrabberBuilder()
        {
        }

        /// <summary>
        /// Creates a new builder.
        /// </summary>
        public static IGrabberBuilder New()
            => new GrabberBuilder();

        public IGrabberBuilder Add(IGrabber grabber)
        {
            _grabbers.Add(grabber);
            return this;
        }

        public IGrabberBuilder Add<T>() where T : IGrabber
        {
            var grabber = (IGrabber)Activator.CreateInstance<T>();
            return Add(grabber);
        }

        public IGrabberBuilder UseServices(IGrabberServices services)
        {
            _services = services;
            return this;
        }

        public IGrabber Build()
        {
            if (_services == null)
                throw new InvalidOperationException($"An instance of {nameof(IGrabberServices)} must be set.");

            return new MultiGrabber(_grabbers, _services);
        }
    }
}
