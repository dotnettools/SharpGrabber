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
using Jint.Runtime.Interop;

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

        /// <summary>
        /// Gets or sets the memory limit for script. A value of 0 represents no limit. Default value is 10 MiB.
        /// </summary>
        public long MemoryLimit { get; set; } = 10_485_760;

        /// <summary>
        /// Gets or sets the maximum allowed time for the script to execute. <see cref="TimeSpan.Zero"/> represents no limit.
        /// Default value is 30 seconds.
        /// </summary>
        public TimeSpan ExecutionTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the call recursion limit. A value of 0 represents no limit. Default value is 256.
        /// </summary>
        public int RecursionLimit { get; set; } = 256;

        /// <summary>
        /// Clears execution limits.
        /// </summary>
        public void SetNoLimits()
        {
            MemoryLimit = 0;
            ExecutionTimeout = TimeSpan.Zero;
            RecursionLimit = 0;
        }

        public async Task<IGrabber> InterpretAsync(IGrabberRepositoryScript script, IGrabberScriptSource source,
            int apiVersion,
            GrabberScriptInterpretOptions options, CancellationToken cancellationToken)
        {
            if (script == null)
                throw new ArgumentNullException(nameof(script));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var engine = CreateEngine(apiVersion, cancellationToken);
            var scriptSource = await source.GetSourceAsync().ConfigureAwait(false);

            var hostObject = _interpreterApiService.GetHostObject(apiVersion, _grabberServices);
            DefineHostObjectOnScript(engine, hostObject);
            DefineAdditionalExposedData(engine, options.ExposedData);
            engine.Execute(scriptSource);

            var processedScript = _interpreterApiService.ProcessResult(apiVersion, hostObject);

            return new JintGrabber(processedScript, script.Name, _grabberServices);
        }

        /// <summary>
        /// Configures Jint engine options.
        /// </summary>
        protected virtual void ConfigureEngine(Options options, int apiVersion, CancellationToken cancellationToken)
        {
            options.SetTypeConverter(engine => ConfigureTypeConverter(engine, apiVersion));
            options.CancellationToken(cancellationToken);
            if (MemoryLimit > 0)
                options.LimitMemory(MemoryLimit);
            if (ExecutionTimeout > TimeSpan.Zero)
                options.TimeoutInterval(ExecutionTimeout);
            if (RecursionLimit > 0)
                options.LimitRecursion(RecursionLimit);
        }

        protected virtual Jint.Runtime.Interop.ITypeConverter ConfigureTypeConverter(Engine engine, int apiVersion)
        {
            // var converter = _interpreterApiService.GetTypeConverter(apiVersion);
            // var multiTypeConverter =
            //     JintMultiTypeConverter.CreateDefault(engine, new ConvertEx.ITypeConverter[] {converter});
            // return multiTypeConverter;
            return new DefaultTypeConverter(engine);
        }

        private static void DefineAdditionalExposedData(Engine engine,
            IEnumerable<KeyValuePair<string, object>> exposedData)
        {
            if (exposedData == null)
                return;

            foreach (var exposedPair in exposedData)
            {
                engine.SetValue(exposedPair.Key, exposedPair.Value);
            }
        }

        private Jint.Engine CreateEngine(int apiVersion, CancellationToken cancellationToken)
        {
            var engine = new Engine((engine, options) => ConfigureEngine(options, apiVersion, cancellationToken));
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

            public JintGrabber(ProcessedGrabScript processedGrabScript, string name,
                IGrabberServices grabberServices) : base(
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