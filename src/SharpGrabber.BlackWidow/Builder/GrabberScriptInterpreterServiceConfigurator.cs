using DotNetTools.ConvertEx;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.TypeConversion;
using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Builds <see cref="IGrabberScriptInterpreterService"/>.
    /// </summary>
    internal class GrabberScriptInterpreterServiceConfigurator : IGrabberScriptInterpreterServiceConfigurator
    {
        private readonly Dictionary<GrabberScriptType, Func<GrabberScriptInterpreterActivationContext, IGrabberScriptInterpreter>> _interpreterFactories = new();
        private Func<GrabberScriptInterpreterApiServiceActivationContext, IInterpreterApiService> _apiServiceFactory;
        private IGrabberServices _grabberServices;
        private IScriptHost _scriptHost;
        private IGrabbedTypeCollection _grabbedTypeCollection;
        private ITypeConverter _typeConverter;

        public IGrabberScriptInterpreterServiceConfigurator UseGrabberServices(IGrabberServices grabberServices)
        {
            _grabberServices = grabberServices;
            return this;
        }

        public IGrabberScriptInterpreterServiceConfigurator UseScriptHost(IScriptHost scriptHost)
        {
            _scriptHost = scriptHost;
            return this;
        }

        public IGrabberScriptInterpreterServiceConfigurator UseGrabbedTypeCollection(IGrabbedTypeCollection grabbedTypeCollection)
        {
            _grabbedTypeCollection = grabbedTypeCollection;
            return this;
        }

        public IGrabberScriptInterpreterServiceConfigurator UseTypeConverter(ITypeConverter typeConverter)
        {
            _typeConverter = typeConverter;
            return this;
        }

        public IGrabberScriptInterpreterServiceConfigurator SetApiService(Func<GrabberScriptInterpreterApiServiceActivationContext, IInterpreterApiService> apiServiceFactory)
        {
            _apiServiceFactory = apiServiceFactory;
            return this;
        }

        public IGrabberScriptInterpreterServiceConfigurator AddInterpreter(GrabberScriptType scriptType,
            Func<GrabberScriptInterpreterActivationContext, IGrabberScriptInterpreter> interpreterFactory)
        {
            _interpreterFactories[scriptType] = interpreterFactory;
            return this;
        }

        public IGrabberScriptInterpreterService Build()
        {
            if (_apiServiceFactory == null)
                this.SetDefaultApiService();
            if (_apiServiceFactory == null)
                throw new InvalidOperationException("Interpreter API service is unspecified.");

            var grabberServies = _grabberServices ?? GrabberServices.Default;
            var scriptHost = _scriptHost ?? new ScriptHost();
            var grabbedTypeCollection = _grabbedTypeCollection ?? new GrabbedTypeCollection();
            var typeConverter = _typeConverter ?? TypeConverters.Default;
            var apiServiceContext = new GrabberScriptInterpreterApiServiceActivationContext(grabberServies, scriptHost, grabbedTypeCollection, typeConverter);
            var apiService = _apiServiceFactory.Invoke(apiServiceContext);
            var interpreterContext = new GrabberScriptInterpreterActivationContext(apiService, grabberServies, scriptHost);

            var service = new GrabberScriptInterpreterService();
            foreach (var pair in _interpreterFactories)
            {
                var interpreter = pair.Value(interpreterContext);
                service.Register(pair.Key, interpreter);
            }
            return service;
        }
    }
}
