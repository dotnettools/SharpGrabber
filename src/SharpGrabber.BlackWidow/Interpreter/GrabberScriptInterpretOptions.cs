using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter
{
    /// <summary>
    /// Defines different options for <see cref="IGrabberScriptInterpreter.InterpretAsync"/>.
    /// </summary>
    public struct GrabberScriptInterpretOptions
    {
        /// <summary>
        /// Gets the default instance of options.
        /// </summary>
        public static readonly GrabberScriptInterpretOptions Default = new();

        /// <summary>
        /// Gets or sets the additional data exposed to the script, besides the host object.
        /// </summary>
        public IDictionary<string, object> ExposedData { get; set; }
    }
}
