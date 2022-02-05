using DotNetTools.SharpGrabber.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    public class GrabberAuthenticationServiceBuilder : IGrabberAuthenticationServiceBuilder
    {
        private IGrabberAuthenticationStore _store;

        private GrabberAuthenticationServiceBuilder()
        {
        }

        /// <summary>
        /// Creates a new builder.
        /// </summary>
        public static IGrabberAuthenticationServiceBuilder New()
            => new GrabberAuthenticationServiceBuilder();

        public IGrabberAuthenticationServiceBuilder UseStore(IGrabberAuthenticationStore store)
        {
            _store = store;
            return this;
        }

        public IGrabberAuthenticationService Build()
        {
            return new GrabberAuthenticationService(_store);
        }
    }
}
