using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Exceptions
{
    public class ScriptInterpretException : BlackWidowException
    {
        public ScriptInterpretException() : this("Script interpret error.")
        {
        }

        public ScriptInterpretException(string message) : base(message)
        {
        }

        public ScriptInterpretException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScriptInterpretException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
