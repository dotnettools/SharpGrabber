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

        static GrabberServices()
        {
            Default.RegisterBuiltInGrabbedTypes();
        }

        private readonly Func<HttpClient> _httpClientProvider;
        private readonly Dictionary<string, Type> _grabbedTypes = new(StringComparer.InvariantCultureIgnoreCase);

        public GrabberServices(Func<HttpClient> httpClientProvider = null, IMimeService mime = null)
        {
            _httpClientProvider = httpClientProvider ?? GetGlobalHttpClient;
            Mime = mime ?? DefaultMimeService.Instance;
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
        /// Scans the primary SharpGrabber assembly for types implementing <see cref="IGrabbed"/>.
        /// </summary>
        public void RegisterBuiltInGrabbedTypes()
        {
            var types = typeof(GrabberServices).Assembly
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IGrabbed)));

            foreach (var type in types)
                RegisterGrabbedType(type);
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
            => _grabbedTypes.GetOrDefault(grabbedId);

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
            if (targetType == typeof(string))
            {
                newValue = value.ToString();
                return true;
            }
            if (targetType == typeof(Uri))
            {
                newValue = new Uri(value.ToString());
                return true;
            }
            if (targetType.IsEnum)
            {
                newValue = Enum.Parse(targetType, value.ToString(), true);
                return true;
            }

            newValue = value;
            return false;
        }
    }
}
