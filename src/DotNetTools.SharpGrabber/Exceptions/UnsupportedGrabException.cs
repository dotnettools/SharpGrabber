using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Exceptions
{
    /// <summary>
    /// May be thrown when grabbing of a URI is not supported by a grabber or multi-grabber.
    /// </summary>
    public class UnsupportedGrabException : GrabException
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public UnsupportedGrabException() : this("The specified URI is not supported for grabbing.")
        {
        }

        public UnsupportedGrabException(string message) : base(message)
        {
        }

        public UnsupportedGrabException(string message, Exception innerException) : base(message, innerException)
        {
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
