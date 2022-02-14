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

        public string Get(string grabberId)
        {
            return _map.TryGetValue(grabberId, out var value) ? value : null;
        }

        public string Get(GrabberAuthenticationRequest request)
        {
            return Get(request.Grabber.StringId);
        }

        public void Set(string grabberId, string state)
        {
            _map[grabberId] = state;
        }

        public void Set(GrabberAuthenticationRequest request, string state)
        {
            Set(request.Grabber.StringId, state);
        }

        public void Delete(string grabberId)
        {
            _map.Remove(grabberId);
        }
    }
}
