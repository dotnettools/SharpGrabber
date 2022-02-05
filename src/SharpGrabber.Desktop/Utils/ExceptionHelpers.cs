using System;
using System.Collections.Generic;
using System.Text;

namespace SharpGrabber.Desktop.Utils
{
    internal static class ExceptionHelpers
    {
        public static Exception FindInnerMostException(this Exception exception)
        {
            if (exception == null)
                return null;
            while (exception.InnerException != null)
                exception = exception.InnerException;
            return exception;
        }
    }
}
