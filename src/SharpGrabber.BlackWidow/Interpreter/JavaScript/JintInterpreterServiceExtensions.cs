using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using System;
using System.Collections.Generic;
using System.Text;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript
{
    /// <summary>
    /// Provides extension methods for <see cref="IGrabberScriptInterpreterService"/>.
    /// </summary>
    public static class JintInterpreterServiceExtensions
    {
        /// <summary>
        /// Registers Jint as the JavaScript interpreter.
        /// </summary>
        public static void RegisterJint(this IGrabberScriptInterpreterService interpreterService, IInterpreterApiService apiService,
            IGrabberServices grabberServices, IScriptHost scriptHost)
        {
            interpreterService.Register(GrabberScriptType.JavaScript, new JintJavaScriptInterpreter(apiService, grabberServices, scriptHost));
        }
    }
}
