using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using Jint.Native;
using Jint;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Interpreter.JavaScript
{
    public class JintUrlTests
    {
        [Fact]
        public void Test_URLParts()
        {
            var engine = CreateEngine();
            var url = engine.GetValue("URL");
            var result = engine.Invoke(url, new JsString("https://user:pass@site.com:1234/serve?id=85#someid"));
            object Get(string name)
                => result.Get(new JsString(name)).ToObject();
            Assert.Equal("#someid", Get("hash"));
            Assert.Equal("site.com:1234", Get("host"));
            Assert.Equal("site.com", Get("hostname"));
            Assert.Equal("https://user:pass@site.com:1234/serve?id=85#someid", Get("href"));
            Assert.Equal("https://site.com:1234", Get("origin"));
            Assert.Equal("user", Get("username"));
            Assert.Equal("pass", Get("password"));
            Assert.Equal("/serve", Get("pathname"));
            Assert.Equal("1234", Get("port"));
            Assert.Equal("https:", Get("protocol"));
            Assert.Equal("?id=85", Get("search"));
        }
        
        private static Engine CreateEngine()
        {
            var engine = new Engine();
            var host = new JintJavaScriptHost(engine, new ScriptHost());
            host.Apply(engine);
            return engine;
        }
    }
}
