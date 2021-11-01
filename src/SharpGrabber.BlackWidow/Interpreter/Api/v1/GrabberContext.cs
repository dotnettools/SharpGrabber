using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class GrabberContext
    {
        public Func<string, bool> Supports { get; set; }

        public Func<GrabRequest, Task<GrabResponse>> Grab { get; set; }
    }
}
