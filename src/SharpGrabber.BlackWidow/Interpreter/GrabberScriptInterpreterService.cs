using System;
using System.Collections.Generic;
using System.Text;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter
{
    /// <summary>
    /// Default implementation for <see cref="IGrabberScriptInterpreterService"/>
    /// </summary>
    public class GrabberScriptInterpreterService : IGrabberScriptInterpreterService
    {
        private readonly Dictionary<GrabberScriptType, IGrabberScriptInterpreter> _interpreters = new();

        public void Register(GrabberScriptType scriptType, IGrabberScriptInterpreter interpreter)
        {
            _interpreters[scriptType] = interpreter;
        }

        public IGrabberScriptInterpreter GetInterpreter(GrabberScriptType scriptType)
        {
            return _interpreters.GetOrDefault(scriptType);
        }
    }
}
