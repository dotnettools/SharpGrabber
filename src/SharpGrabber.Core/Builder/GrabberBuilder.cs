using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    public class GrabberBuilder : IGrabberBuilder
    {
        private readonly HashSet<IGrabber> _grabbers = new();

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

        public IGrabber Build()
            => new MultiGrabber(_grabbers);
    }
}
