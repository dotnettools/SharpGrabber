using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.Exceptions;
using Jint;
using Jint.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class JintJavaScriptHost
    {
        private readonly Engine _engine;
        private readonly IScriptHost _host;

        public JintJavaScriptHost(Engine engine, Host.IScriptHost scriptHost)
        {
            _engine = engine;
            _host = scriptHost;
        }

        public void Apply(Engine engine)
        {
            engine.SetValue(new JsString("GrabException"), typeof(GrabException));

            engine.SetValue(new JsString("alert"), (Action<object>)_host.Alert);
            engine.SetValue(new JsString("console"), new ConsoleContext(this));
            engine.SetValue(new JsString("URL"), typeof(URL));
        }

        #region console
        private class ConsoleContext
        {
            private IScriptHost _host;

            public ConsoleContext(JintJavaScriptHost self)
            {
                _host = self._host;
            }

            public void log(params object[] args)
            {
                _host.Log(new ConsoleLog(ConsoleLogLevel.Log, args));
            }

            public void debug(params object[] args)
            {
                _host.Log(new ConsoleLog(ConsoleLogLevel.Debug, args));
            }

            public void error(params object[] args)
            {
                _host.Log(new ConsoleLog(ConsoleLogLevel.Error, args));
            }

            public void info(params object[] args)
            {
                _host.Log(new ConsoleLog(ConsoleLogLevel.Info, args));
            }

            public void trace(params object[] args)
            {
                _host.Log(new ConsoleLog(ConsoleLogLevel.Trace, args));
            }

            public void warning(params object[] args)
            {
                _host.Log(new ConsoleLog(ConsoleLogLevel.Warning, args));
            }
        }
        #endregion

        #region URL
        private class URL
        {
            public URL(string url, string @base)
            {
                Uri uri;
                if (string.IsNullOrEmpty(@base))
                    uri = new(url);
                else
                    uri = new(new Uri(@base), url);
                hash = uri.Fragment;
                host = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
                hostname = uri.Host;
                href = uri.ToString();
                origin = $"{uri.Scheme}://{host}";
                pathname = uri.LocalPath;
                port = uri.Port.ToString();
                protocol = uri.Scheme + ':';
                search = uri.Query;
                if (!string.IsNullOrEmpty(uri.UserInfo))
                {
                    var userPass = uri.UserInfo.Split(new[] { ':' }, 2);
                    username = userPass[0];
                    password = userPass[1];
                }
            }

            public URL(string url) : this(url, null) { }

            public string hash { get; }
            public string host { get; }
            public string hostname { get; }
            public string href { get; }
            public string origin { get; }
            public string username { get; }
            public string password { get; }
            public string pathname { get; }
            public string port { get; }
            public string protocol { get; }
            public string search { get; }
        }
        #endregion
    }
}
