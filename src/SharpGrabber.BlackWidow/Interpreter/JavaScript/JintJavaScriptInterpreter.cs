using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Internal;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
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
            _interpreterApiService = interpreterApiService;
            _grabberServices = grabberServices;
            _scriptHost = host;
        }

        /// <summary>
        /// Gets or sets the name of the main function.
        /// </summary>
        public string MainFunctionName { get; set; } = "main";

        public async Task<IGrabber> InterpretAsync(IGrabberScriptSource script, int apiVersion)
        {
            var engine = CreateEngine();
            var scriptSource = await script.GetSourceAsync().ConfigureAwait(false);

            var hostObject = _interpreterApiService.GetHostObject(apiVersion, _grabberServices);
            DefineHostObjectOnScript(engine, hostObject);
            engine.Execute(scriptSource);

            var processedScript = _interpreterApiService.ProcessResult(apiVersion, hostObject);
            var b1 = processedScript.Supports(new Uri("http://jwiqdvajavdi"));
            var b2 = processedScript.Supports(new Uri("http://fb.com/qwesd/qwrr?qwet=5&asd=bsd"));
            return null;
        }

        private Jint.Engine CreateEngine()
        {
            var engine = new Jint.Engine(o => { });
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

        private class JavaScriptGrabber : GrabberBase
        {
            public JavaScriptGrabber(string stringId, string name, IGrabberServices grabberServices) : base(
                grabberServices)
            {
                StringId = stringId;
                Name = name;
            }

            public override string StringId { get; }

            public override string Name { get; }

            public override bool Supports(Uri uri)
            {
                throw new NotImplementedException();
            }

            protected override Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken,
                GrabOptions options, IProgress<double> progress)
            {
                throw new NotImplementedException();
            }
        }
    }
}