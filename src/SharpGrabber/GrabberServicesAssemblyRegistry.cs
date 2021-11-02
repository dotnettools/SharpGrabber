using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Allows assemblies to register themselves for scanning.
    /// </summary>
    public static class GrabberServicesAssemblyRegistry
    {
        private static readonly ConcurrentBag<Assembly> _assemblies = new() { typeof(GrabberServicesAssemblyRegistry).Assembly };

        /// <summary>
        /// Gets the registered assemblies.
        /// </summary>
        public static IEnumerable<Assembly> Assemblies => _assemblies.AsEnumerable();

        /// <summary>
        /// Registers an assembly.
        /// </summary>
        public static void Register(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }
    }
}
