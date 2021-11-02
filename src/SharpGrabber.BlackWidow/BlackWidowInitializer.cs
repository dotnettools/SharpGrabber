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
            // nothing should be done here,
            // the static constructor would run once.
        }

        private static void EnsureLoaded(params Type[] types)
        {
            foreach (var type in types)
            {
                // create a dummy instance just to ensure the type is loaded
                var o = Activator.CreateInstance(type);
                if (o is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }
}
