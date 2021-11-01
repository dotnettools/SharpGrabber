using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    public interface IInterpreterApiService
    {
        object GetHostObject(int apiVersion, IGrabberServices grabberServices);

        ProcessedGrabScript ProcessResult(int apiVersion, object hostObject);
    }
}
