using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript
{
    /// <summary>
    /// Defines a grabber script interpreter that internally uses Jurassic.
    /// </summary>
    public class JurassicJavaScriptInterpreter : IGrabberScriptInterpreter
    {
        private readonly IGrabberServices _grabberServices;

        public Task<IGrabber> InterpretAsync(IGrabberScriptSource script)
        {
            var engine = CreateEngine();
            return Task.FromResult<IGrabber>(null);
        }

        private Jurassic.ScriptEngine CreateEngine()
        {
            var engine = new Jurassic.ScriptEngine
            {
                EnableExposedClrTypes = false,
            };
            return engine;
        }

        private class JavaScriptGrabber : GrabberBase
        {
            public JavaScriptGrabber(string stringId, string name, IGrabberServices grabberServices) : base(grabberServices)
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

            protected override Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
            {
                throw new NotImplementedException();
            }
        }
    }
}
