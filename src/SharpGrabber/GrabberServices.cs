using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Internal;

namespace DotNetTools.SharpGrabber
{
    public class GrabberServices : IGrabberServices
    {
        public static readonly GrabberServices Default = new();

        private readonly bool _autoRegisterBuiltInGrabbedTypes;
        private readonly Func<HttpClient> _httpClientProvider;
        private readonly Dictionary<string, Type> _grabbedTypes = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly HashSet<Assembly> _scannedAssemblies = new();

        public GrabberServices(Func<HttpClient> httpClientProvider = null, IMimeService mime = null, bool registerBuiltInGrabbedTypes = true)
        {
            _autoRegisterBuiltInGrabbedTypes = registerBuiltInGrabbedTypes;
            _httpClientProvider = httpClientProvider ?? GetGlobalHttpClient;
            Mime = mime ?? DefaultMimeService.Instance;
            if (registerBuiltInGrabbedTypes)
                RegisterBuiltInGrabbedTypes();
        }
        public IMimeService Mime { get; }

        public HttpClient GetClient()
            => _httpClientProvider();

        private static HttpClient _globalHttpClient;

        private static HttpClient GetGlobalHttpClient()
        {
            if (_globalHttpClient == null)
                lock (typeof(GrabberBase))
                {
                    if (_globalHttpClient == null)
                    {
                        var defaultProvider = new DefaultGlobalHttpProvider();
                        _globalHttpClient = defaultProvider.GetClient();
                    }
                }
            return _globalHttpClient;
        }

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

        public object ChangeType(object value, Type targetType)
        {
            // init
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));
            if (value == null)
                return null;

            // test same type
            var valueType = value.GetType();
            if (valueType == targetType)
                return value;

            // try internal conversion
            if (ChangeTypeInternal(value, targetType, out var result))
                return result;

            // fallback
            return Convert.ChangeType(value, targetType);
        }

        protected virtual bool ChangeTypeInternal(object value, Type targetType, out object newValue)
        {
            var t = targetType;
            var nullable = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (nullable)
                t = t.GetGenericArguments()[0];

            if (t == typeof(string))
            {
                newValue = value.ToString();
                return true;
            }
            if (t == typeof(Uri))
            {
                newValue = new Uri(value.ToString());
                return true;
            }
            if (t == typeof(TimeSpan))
            {
                var milliseconds = (double)Convert.ChangeType(value, typeof(double));
                newValue = TimeSpan.FromMilliseconds(milliseconds);
                return true;
            }
            if (t.IsEnum)
            {
                newValue = Enum.Parse(t, value.ToString(), true);
                return true;
            }

            newValue = value;
            return false;
        }
    }
}
