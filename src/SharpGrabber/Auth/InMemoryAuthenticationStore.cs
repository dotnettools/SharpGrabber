using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    internal class InMemoryAuthenticationStore : IGrabberAuthenticationStore
    {
        private readonly Dictionary<string, string> _map = new();

        public string Get(string key, string @default = null)
        {
            return _map.TryGetValue(key, out var value) ? value : @default;
        }

        public void Set(string key, string value)
        {
            _map[key] = value;
        }

        public void Delete(string key)
        {
            _map.Remove(key);
        }
    }
}
