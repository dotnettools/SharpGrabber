using DotNetTools.ConvertEx;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Provides references to services used when activating an interpreter.
    /// </summary>
    public class GrabberScriptInterpreterActivationContext
    {
        public GrabberScriptInterpreterActivationContext(IInterpreterApiService apiService, IGrabberServices grabberServices, IScriptHost scripHost)
        {
            ApiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            GrabberServices = grabberServices ?? throw new ArgumentNullException(nameof(grabberServices));
            ScriptHost = scripHost ?? throw new ArgumentNullException(nameof(scripHost));
        }

        /// <summary>
        /// Gets the interpreter API service.
        /// </summary>
        public IInterpreterApiService ApiService { get; }

        /// <summary>
        /// Gets the grabber services.
        /// </summary>
        public IGrabberServices GrabberServices { get; }

        /// <summary>
        /// Gets the script host.
        /// </summary>
        public IScriptHost ScriptHost { get; }
    }
}
