using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Host
{
    /// <summary>
    /// Implements <see cref="IScriptHost"/> with events.
    /// </summary>
    public class ScriptHost : IScriptHost
    {
        public event Action<object> OnAlert;
        public event Action<ConsoleLog> OnLog;

        public void Alert(object input)
        {
            OnAlert?.Invoke(input);
        }

        public void Log(ConsoleLog log)
        {
            OnLog?.Invoke(log);
        }
    }
}
