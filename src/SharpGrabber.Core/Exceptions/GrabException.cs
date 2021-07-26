using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetTools.SharpGrabber.Exceptions
{
    /// <summary>
    /// Base and default class for any exception related to the process of grabbing.
    /// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class GrabException : SharpGrabberException
    {
        public GrabException() : this("Failed to grab the target.")
        {
        }

        public GrabException(string message) : base(message)
        {
        }

        public GrabException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
