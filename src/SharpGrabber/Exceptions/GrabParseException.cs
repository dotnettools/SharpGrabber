using System;

namespace DotNetTools.SharpGrabber.Exceptions
{
    /// <summary>
    /// Represents grab exceptions that occur when a grabber is trying to parse information on the target page
    /// and fails to locate or fetch necessary data from them. 
    /// </summary>
    /// <remarks>
    /// This exception occurs often due to changes on the target server. Therefore, messages for this type of
    /// exception are usually technical and not intended to be displayed to final user; use them for diagnosing
    /// purposes only.
    /// </remarks>
    public class GrabParseException : GrabException
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GrabParseException() : base("Failed to parse information from the target link.")
        {
        }

        public GrabParseException(string message) : base(message)
        {
        }

        public GrabParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}