using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using Jint;
using SharpGrabber.BlackWidow.Tests.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Interpreter.JavaScript
{
    public class JavaScriptInterpreterTests
    {
        [Fact]
        public async Task Test_Simple()
        {
            var html = AssetsAccessor.GetText("Html/SamplePage.html");
            var collection = new GrabbedTypeCollection();
            var apiService = new DefaultInterpreterApiService(GrabberServices.Default, collection, ApiTypeConverter.Default);
            var host = new ScriptHost();
            host.OnAlert += o =>
            {
                Debugger.Break();
            };
            host.OnLog += log =>
            {
                Debugger.Break();
            };
            var i = new JintJavaScriptInterpreter(apiService, GrabberServices.Default, host);
            var script = new GrabberRepositoryScript();
            var src = GrabberScriptSource.FromFile(@"");
            var options = new GrabberScriptInterpretOptions
            {
                ExposedData = new Dictionary<string, object>
                {
                    {
                        "data",
                        new
                        {
                            samplePage = html,
                        }
                    }
                }
            };
            var grabber = await i.InterpretAsync(script, src, 1, options);

            var result = await grabber.GrabAsync(new Uri(@""));
        }
    }
}