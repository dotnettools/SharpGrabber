using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Describes the response to an authentication request.
    /// </summary>
    public readonly struct GrabberAuthenticationResult
    {
        /// <summary>
        /// Gets the empty instance of <see cref="GrabberAuthenticationResult"/> with NULL state.
        /// </summary>
        public static readonly GrabberAuthenticationResult Empty = new(null);

        /// <summary>
        /// The state object
        /// </summary>
        public readonly object State;

        public GrabberAuthenticationResult(object state)
        {
            State = state;
        }
    }

    /// <summary>
    /// Defines extension methods on <see cref="GrabberAuthenticationResult"/>.
    /// </summary>
    public static class GrabberAuthenticationResultExtensions
    {
        /// <summary>
        /// Gets the state object of type <typeparamref name="T"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T State<T>(this GrabberAuthenticationResult result)
            => (T)result.State;
    }

}
