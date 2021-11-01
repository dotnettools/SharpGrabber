using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public delegate bool GrabberSupportsDelegate(string url);

    public delegate void GrabberGrabDelegate(GrabRequest request, GrabResponse response, Action resolve, Action reject);

    public delegate Task GrabberGrabAsyncDelegate(GrabRequest request, GrabResponse response);
}
