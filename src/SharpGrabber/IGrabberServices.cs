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
        /// Gets an <see cref="HttpClient"/>.
        /// </summary>
        /// <remarks>
        /// The result <see cref="HttpClient"/> MUST NOT be disposed; otherwise it will break the functionality.
        /// </remarks>
        HttpClient GetClient();
    }
}
