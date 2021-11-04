using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter
{
    /// <summary>
    /// Interprets a script as a grabber.
    /// </summary>
    public interface IGrabberScriptInterpreter
    {
        /// <summary>
        /// Loads the specified script source and interprets it as a grabber.
        /// </summary>
        Task<IGrabber> InterpretAsync(IGrabberRepositoryScript script, IGrabberScriptSource source, int apiVersion, GrabberScriptInterpretOptions options);
    }

    public static class GrabberScriptInterpreterExtensions
    {
        /// <summary>
        /// Loads the specified script source and interprets it as a grabber.
        /// </summary>
        public static Task<IGrabber> InterpretAsync(this IGrabberScriptInterpreter interpreter, IGrabberRepositoryScript script,
            IGrabberScriptSource source, int apiVersion)
        {
            return interpreter.InterpretAsync(script, source, apiVersion, GrabberScriptInterpretOptions.Default);
        }
    }
}
