using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Host
{
    /// <summary>
    /// Defines handlers for various operations on the script host.
    /// </summary>
    public interface IScriptHost
    {
        void Alert(object input);

        void Log(ConsoleLog log);
    }
}
