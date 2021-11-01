using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Exceptions
{
    public class ScriptApiVersionMismatchException : BlackWidowException
    {
        public ScriptApiVersionMismatchException()
        {
        }

        public ScriptApiVersionMismatchException(string message) : base(message)
        {
        }

        public ScriptApiVersionMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScriptApiVersionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
