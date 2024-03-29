﻿using System;
using System.Runtime.Serialization;

namespace DotNetTools.SharpGrabber.Exceptions
{
    /// <summary>
    /// Base exception class for all SharpGrabber exceptions
    /// </summary>
    public class SharpGrabberException : Exception
    {

        public SharpGrabberException()
        {
        }

        public SharpGrabberException(string message) : base(message)
        {
        }

        public SharpGrabberException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SharpGrabberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}
