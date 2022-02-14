using DotNetTools.SharpGrabber.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Represents a single purpose handler.
    /// </summary>
    public interface IGrabberAuthenticationHandler
    {
        /// <summary>
        /// Tests if this handler supports the specified grabber.
        /// </summary>
        bool Supports(IGrabber grabber);

        /// <summary>
        /// Tries to authenticate the grabber client.
        /// </summary>
        /// <returns>The state object. A return value of NULL indicates general failure.</returns>
        /// <remarks>
        /// This method may throw any type of exception in case of failure, or just return NULL.
        /// </remarks>
        Task<object> AuthenticateAsync(GrabberAuthenticationRequest request);

        /// <summary>
        /// Serializes a state object.
        /// </summary>
        string SerializeState(object state);

        /// <summary>
        /// Deserializes a state object.
        /// </summary>
        object DeserializeState(string state);
    }
}
