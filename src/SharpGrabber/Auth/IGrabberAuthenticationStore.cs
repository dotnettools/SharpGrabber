using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Provides a store for authentication information.
    /// </summary>
    public interface IGrabberAuthenticationStore
    {
        /// <summary>
        /// Gets the authentication state for a grabber.
        /// </summary>
        string Get(string grabberId);

        /// <summary>
        /// Gets the authentication state for an authentication request.
        /// </summary>
        string Get(GrabberAuthenticationRequest request);

        /// <summary>
        /// Sets the state defined by <paramref name="grabberId"/>.
        /// </summary>
        void Set(string grabberId, string state);

        /// <summary>
        /// Sets the state defined by <paramref name="request"/>.
        /// </summary>
        void Set(GrabberAuthenticationRequest request, string state);

        /// <summary>
        /// Deletes the entry associated with <paramref name="grabberId"/> if exists.
        /// </summary>
        void Delete(string grabberId);
    }
}
