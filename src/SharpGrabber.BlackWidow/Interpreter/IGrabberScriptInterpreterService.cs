using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter
{
    /// <summary>
    /// Manages grabber script interpreters.
    /// </summary>
    public interface IGrabberScriptInterpreterService
    {
        /// <summary>
        /// Registers <paramref name="interpreter"/> for <paramref name="scriptType"/>.
        /// </summary>
        void Register(GrabberScriptType scriptType, IGrabberScriptInterpreter interpreter);

        /// <summary>
        /// Gets the proper interpreter for <paramref name="scriptType"/>, if available.
        /// </summary>
        IGrabberScriptInterpreter GetInterpreter(GrabberScriptType scriptType);
    }
}
