using DotNetTools.SharpGrabber.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Gets configured to provide certain services to grabbers.
    /// </summary>
    public interface IGrabberServices
    {
        /// <summary>
        /// Gets the mime service.
        /// </summary>
        IMimeService Mime { get; }

        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        IGrabberAuthenticationService Authentication { get; }

        /// <summary>
        /// Gets an <see cref="HttpClient"/>.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="HttpClient"/> must NOT be disposed; otherwise it would break the functionality.
        /// </remarks>
        HttpClient GetClient();
    }
}
