using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Http;
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
            Http = new ApiHttpContext(grabberServices);
        }

        public ApiGrabberContext Grabber { get; } = new ApiGrabberContext();

        public ApiHttpContext Http { get; }
    }
}
