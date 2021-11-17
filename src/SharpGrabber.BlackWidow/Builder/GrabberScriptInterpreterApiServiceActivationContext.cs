using DotNetTools.ConvertEx;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Provides references to services used when activating an interpreter API service.
    /// </summary>
    public class GrabberScriptInterpreterApiServiceActivationContext
    {
        public GrabberScriptInterpreterApiServiceActivationContext(IGrabberServices grabberServices, IScriptHost scriptHost,
            IGrabbedTypeCollection grabbedTypes, ITypeConverter typeConverter)
        {
            GrabberServices = grabberServices ?? throw new ArgumentNullException(nameof(grabberServices));
            ScriptHost = scriptHost ?? throw new ArgumentNullException(nameof(scriptHost));
            GrabbedTypes = grabbedTypes ?? throw new ArgumentNullException(nameof(grabbedTypes));
            TypeConverter = typeConverter ?? throw new ArgumentNullException(nameof(typeConverter));
        }

        /// <summary>
        /// Gets the grabber services.
        /// </summary>
        public IGrabberServices GrabberServices { get; }

        /// <summary>
        /// Gets the script host.
        /// </summary>
        public IScriptHost ScriptHost { get; }

        /// <summary>
        /// Gets the collection of grabbed types.
        /// </summary>
        public IGrabbedTypeCollection GrabbedTypes { get; }

        /// <summary>
        /// Gets the type converter.
        /// </summary>
        public ITypeConverter TypeConverter { get; }
    }
}
