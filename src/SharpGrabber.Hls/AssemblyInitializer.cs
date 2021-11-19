using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Hls
{
    internal class AssemblyInitializer
    {
        static AssemblyInitializer()
        {
            GrabberServicesAssemblyRegistry.Register(typeof(HlsGrabber).Assembly);
        }
    }
}
