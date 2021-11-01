using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var apiService = new DefaultInterpreterApiService();
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
            var src = GrabberScriptSource.FromFile(@"C:\Users\Jaweed\Desktop\new 11.js");
            var grabber = await i.InterpretAsync(src, 1);
        }
    }
}