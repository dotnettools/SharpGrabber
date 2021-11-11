using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public delegate bool GrabberSupportsDelegate(string url);

    public delegate bool GrabberGrabDelegate(ApiGrabRequest request, ApiGrabResponse response);

    public delegate Task GrabberGrabAsyncDelegate(ApiGrabRequest request, ApiGrabResponse response);
}
