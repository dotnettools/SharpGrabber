using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetTools.SharpGrabber.Exceptions
{
    /// <summary>
    /// Base and default class for any exception related to the process of grabbing.
    /// </summary>

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

}
