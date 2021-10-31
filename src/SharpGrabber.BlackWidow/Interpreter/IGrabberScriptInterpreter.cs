using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter
{
    public interface IGrabberScriptInterpreter
    {
        Task<IGrabber> InterpretAsync(IGrabberScriptSource script);
    }
}
