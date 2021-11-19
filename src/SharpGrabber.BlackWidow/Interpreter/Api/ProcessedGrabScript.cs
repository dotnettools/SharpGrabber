using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    public class ProcessedGrabScript
    {
        public ProcessedGrabScript(SupportsDelegate supports, GrabDelegate grabAsync)
        {
            Supports = supports;
            GrabAsync = grabAsync;
        }

        public SupportsDelegate Supports { get; }

        public GrabDelegate GrabAsync { get; }
    }
}
