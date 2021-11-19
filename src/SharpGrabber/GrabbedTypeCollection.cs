using DotNetTools.SharpGrabber.Grabbed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Collects types implementing <see cref="IGrabbed"/> and provides access to them as needed.
    /// </summary>
    public class GrabbedTypeCollection : IGrabbedTypeCollection
    {
        private readonly bool _autoRegisterBuiltInGrabbedTypes;
        private readonly Dictionary<string, Type> _grabbedTypes = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly HashSet<Assembly> _scannedAssemblies = new();

        public GrabbedTypeCollection(bool registerBuiltInGrabbedTypes = true)
        {
            _autoRegisterBuiltInGrabbedTypes = registerBuiltInGrabbedTypes;
            if (registerBuiltInGrabbedTypes)
                RegisterBuiltInGrabbedTypes();
        }

        public Type this[string grabbedId] => GetGrabbed(grabbedId);

        /// <summary>
        /// Scans the built-in assemblies for types implementing <see cref="IGrabbed"/>, if not already scanned.
        /// </summary>
        public void RegisterBuiltInGrabbedTypes()
        {
            var newAssemblies = GrabberServicesAssemblyRegistry.Assemblies
                .Where(a => !_scannedAssemblies.Contains(a))
                .ToArray();

            var types = newAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Contains(typeof(IGrabbed)));

            foreach (var type in types)
                RegisterGrabbedType(type);

            foreach (var assembly in newAssemblies)
                _scannedAssemblies.Add(assembly);
        }

        public void RegisterGrabbedType(Type type)
        {
            // test if implements IGrabbed
            if (!type.GetInterfaces().Contains(typeof(IGrabbed)))
                throw new InvalidOperationException($"{type} does not implement {typeof(IGrabbed)}.");

            // get attribute
            var grabbedTypeAttribute = type.GetCustomAttribute<GrabbedTypeAttribute>();
            var id = grabbedTypeAttribute?.Id ?? type.Name.Replace("Grabbed", string.Empty);

            // try detect duplicate identifier
            if (_grabbedTypes.TryGetValue(id, out var oldType) && oldType != type)
                throw new InvalidOperationException($"Duplicate identifiers detected on types {type} and {oldType}.");

            _grabbedTypes[id] = type;
        }

        public Type GetGrabbed(string grabbedId)
        {
            var grabbed = _grabbedTypes.GetOrDefault(grabbedId);
            if (grabbed != null || !_autoRegisterBuiltInGrabbedTypes)
                return grabbed;

            RegisterBuiltInGrabbedTypes();
            return _grabbedTypes.GetOrDefault(grabbedId);
        }
    }
}
