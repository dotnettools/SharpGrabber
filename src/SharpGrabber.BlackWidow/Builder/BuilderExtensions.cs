using DotNetTools.SharpGrabber.BlackWidow.Definitions;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;

namespace DotNetTools.SharpGrabber.BlackWidow
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
