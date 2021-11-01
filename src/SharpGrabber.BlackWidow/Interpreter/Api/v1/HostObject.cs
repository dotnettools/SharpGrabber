using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class HostObject
    {
        public HostObject(IGrabberServices grabberServices)
        {
        }

        public GrabberContext Grabber { get; } = new GrabberContext();
    }
}
