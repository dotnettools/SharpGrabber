using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Defines extension methods for builder and configurator interfaces to work with built-in implementations.
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Registers Jint as the JavaScript interpreter.
        /// </summary>
        public static IGrabberScriptInterpreterServiceConfigurator AddJint(this IGrabberScriptInterpreterServiceConfigurator interpreterService)
        {
            return interpreterService.AddInterpreter(GrabberScriptType.JavaScript, context =>
            {
                return new JintJavaScriptInterpreter(context.ApiService, context.GrabberServices, context.ScriptHost);
            });
        }

        /// <summary>
        /// Configures to use the official API service.
        /// </summary>
        public static IGrabberScriptInterpreterServiceConfigurator SetDefaultApiService(this IGrabberScriptInterpreterServiceConfigurator interpreterService)
        {
            return interpreterService.SetApiService(context => new DefaultInterpreterApiService(context.GrabberServices, context.GrabbedTypes, context.TypeConverter));
        }
    }
}
