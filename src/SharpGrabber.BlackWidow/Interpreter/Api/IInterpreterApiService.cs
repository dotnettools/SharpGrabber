using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    /// <summary>
    /// Interpreter API service
    /// </summary>
    public interface IInterpreterApiService
    {
        /// <summary>
        /// Gets the host object, which will be exposed to the script.
        /// </summary>
        object GetHostObject(int apiVersion, IGrabberServices grabberServices);

        /// <summary>
        /// Processes the result of the call by processing the <paramref name="hostObject"/>.
        /// </summary>
        ProcessedGrabScript ProcessResult(int apiVersion, object hostObject);
    }
}
