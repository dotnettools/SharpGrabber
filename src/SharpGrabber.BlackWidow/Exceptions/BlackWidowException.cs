using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Exceptions
{
    public class BlackWidowException : Exception
    {
        public BlackWidowException()
        {
        }

        public BlackWidowException(string message) : base(message)
        {
        }

        public BlackWidowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BlackWidowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
