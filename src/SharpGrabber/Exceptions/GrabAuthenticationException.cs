using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Exceptions
{
    /// <summary>
    /// Thrown when there's a problem when authenticating a grabber.
    /// </summary>
    public class GrabAuthenticationException : GrabException
    {
        public GrabAuthenticationException() : this("Could not authenticate the grabber.")
        {
        }

        public GrabAuthenticationException(string message) : base(message)
        {
        }

        public GrabAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
