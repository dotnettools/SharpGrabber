using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class ApiGrabberContext
    {
        public GrabberSupportsDelegate Supports { get; set; }

        public GrabberGrabDelegate Grab { get; set; }

        public GrabberGrabAsyncDelegate GrabAsync { get; set; }
    }
}
