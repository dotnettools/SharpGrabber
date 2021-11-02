using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    internal static class BlackWidowInitializer
    {
        static BlackWidowInitializer()
        {
            EnsureLoaded(Hls.HlsGrabber.Initializer);
        }

        public static void Test()
        {
        }

        private static void EnsureLoaded(params Type[] types)
        {
            foreach (var type in types)
            {
                // nothing to do here
                // the framework will automatically load the assembly
                var o = Activator.CreateInstance(type);
                if (o is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}
