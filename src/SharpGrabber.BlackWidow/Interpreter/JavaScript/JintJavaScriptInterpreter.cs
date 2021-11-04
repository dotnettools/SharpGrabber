using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Internal;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using Jint;
using Jint.Native;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript
{
    /// <summary>
    /// Defines a grabber script interpreter that internally uses Jint.
    /// </summary>
    public class JintJavaScriptInterpreter : IGrabberScriptInterpreter
    {
        private readonly IInterpreterApiService _interpreterApiService;
        private readonly IGrabberServices _grabberServices;
        private readonly IScriptHost _scriptHost;

        public JintJavaScriptInterpreter(IInterpreterApiService interpreterApiService, IGrabberServices grabberServices,
            IScriptHost host)
        {
            BlackWidowInitializer.Test();
            _interpreterApiService = interpreterApiService;
            _grabberServices = grabberServices;
            _scriptHost = host;
        }

        /// <summary>
        /// Gets or sets the name of the main function.
        /// </summary>
        public string MainFunctionName { get; set; } = "main";

        public async Task<IGrabber> InterpretAsync(IGrabberRepositoryScript script, IGrabberScriptSource source, int apiVersion,
            GrabberScriptInterpretOptions options)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var engine = CreateEngine();
            var scriptSource = await source.GetSourceAsync().ConfigureAwait(false);

            var hostObject = _interpreterApiService.GetHostObject(apiVersion, _grabberServices);
            DefineHostObjectOnScript(engine, hostObject);
            DefineAdditionalExposedData(engine, options.ExposedData);
            engine.Execute(scriptSource);

            var processedScript = _interpreterApiService.ProcessResult(apiVersion, hostObject);

            return new JintGrabber(processedScript, script.Name, _grabberServices);
        }

        private void DefineAdditionalExposedData(Engine engine, IEnumerable<KeyValuePair<string, object>> exposedData)
        {
            foreach (var exposedPair in exposedData)
            {
                engine.SetValue(exposedPair.Key, exposedPair.Value);
            }
        }

        private Jint.Engine CreateEngine()
        {
            var engine = new Engine(o =>
            {
            });
            var host = new JintJavaScriptHost(engine, _scriptHost);
            host.Apply(engine);
            return engine;
        }

        private void DefineHostObjectOnScript(Jint.Engine engine, object hostObject)
        {
            foreach (var property in hostObject.GetType().GetProperties())
            {
                var val = property.GetValue(hostObject);
                engine.SetValue(new JsString(property.Name.ToCamelCase()), val);
            }
        }

        private class JintGrabber : GrabberBase
        {
            private readonly ProcessedGrabScript _processedGrabScript;

            public JintGrabber(ProcessedGrabScript processedGrabScript, string name, IGrabberServices grabberServices) : base(
                grabberServices)
            {
                _processedGrabScript = processedGrabScript;
                Name = name;
            }

            public override string StringId => null;

            public override string Name { get; }

            public override bool Supports(Uri uri)
                => _processedGrabScript.Supports(uri);

            protected override Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken,
                GrabOptions options, IProgress<double> progress)
                => _processedGrabScript.GrabAsync(uri, cancellationToken, options, progress);
        }
    }
}