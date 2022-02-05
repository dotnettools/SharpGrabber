using DotNetTools.SharpGrabber.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Provides authentication services for grabbers.
    /// </summary>
    public interface IGrabberAuthenticationService
    {
        /// <summary>
        /// Gets the authentication store.
        /// </summary>
        IGrabberAuthenticationStore Store { get; }

        /// <summary>
        /// Registers an authentication handler.
        /// </summary>
        IDisposable RegisterHandler(IGrabberAuthenticationHandler handler);

        /// <summary>
        /// Attempts to authenticate a grabber.
        /// </summary>
        /// <exception cref="GrabAuthenticationException">Thrown if the grabber cannot be authenticated.</exception>
        Task<GrabberAuthenticationResult> RequestAsync(GrabberAuthenticationRequest request);
    }
}
