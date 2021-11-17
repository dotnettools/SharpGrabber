using DotNetTools.ConvertEx;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Configures a <see cref="IGrabberScriptInterpreter"/> for a BlackWidow builder.
    /// </summary>
    public interface IGrabberScriptInterpreterServiceConfigurator
    {
        /// <summary>
        /// Configures the builder to use <paramref name="grabberServices"/>.
        /// </summary>
        IGrabberScriptInterpreterServiceConfigurator UseGrabberServices(IGrabberServices grabberServices);

        /// <summary>
        /// Configures the builder to use <paramref name="scriptHost"/>.
        /// </summary>
        IGrabberScriptInterpreterServiceConfigurator UseScriptHost(IScriptHost scriptHost);

        /// <summary>
        /// Configures the builder to use <paramref name="grabbedTypeCollection"/>.
        /// </summary>
        IGrabberScriptInterpreterServiceConfigurator UseGrabbedTypeCollection(IGrabbedTypeCollection grabbedTypeCollection);

        /// <summary>
        /// Configures the builder to use <paramref name="typeConverter"/>.
        /// </summary>
        IGrabberScriptInterpreterServiceConfigurator UseTypeConverter(ITypeConverter typeConverter);

        /// <summary>
        /// Sets an interpreter API service factory.
        /// </summary>
        IGrabberScriptInterpreterServiceConfigurator SetApiService(Func<GrabberScriptInterpreterApiServiceActivationContext, IInterpreterApiService> apiServiceFactory);

        /// <summary>
        /// Registers an interpreter factory.
        /// </summary>
        IGrabberScriptInterpreterServiceConfigurator AddInterpreter(GrabberScriptType scriptType,
            Func<GrabberScriptInterpreterActivationContext, IGrabberScriptInterpreter> interpreterFactory);

        /// <summary>
        /// Builds a configured instance of <see cref="IGrabberScriptInterpreterService"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown in case of missing information.</exception>
        IGrabberScriptInterpreterService Build();
    }
}
