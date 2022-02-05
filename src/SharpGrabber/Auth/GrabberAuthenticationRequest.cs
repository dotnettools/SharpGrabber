using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Describes an authentication request.
    /// </summary>
    public class GrabberAuthenticationRequest
    {
        public GrabberAuthenticationRequest(IGrabber grabber, object state, CancellationToken cancellationToken)
        {
            Grabber = grabber ?? throw new ArgumentNullException(nameof(grabber));
            CancellationToken = cancellationToken;
            RequestState = state;
        }

        public GrabberAuthenticationRequest(IGrabber grabber, object state = null)
            : this(grabber, state, CancellationToken.None)
        {
        }

        /// <summary>
        /// Gets the grabber requesting to authenticate.
        /// </summary>
        public IGrabber Grabber { get; }

        /// <summary>
        /// Gets the cancellation token associated with this request.
        /// </summary>
        public CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets the request state object.
        /// </summary>
        public object RequestState { get; }
    }

    /// <summary>
    /// Defines extension methods for <see cref="GrabberAuthenticationRequest"/>.
    /// </summary>
    public static class GrabberAuthenticationRequestExtensions
    {
        /// <summary>
        /// Gets the state object of type <typeparamref name="TState"/>.
        /// </summary>
        public static TState GetRequestState<TState>(this GrabberAuthenticationRequest request)
        {
            return (TState)request.RequestState;
        }
    }
}
