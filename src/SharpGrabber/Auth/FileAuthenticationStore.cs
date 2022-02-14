using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    internal class FileAuthenticationStore : IGrabberAuthenticationStore
    {
        private readonly string _path;
        private readonly Dictionary<string, string> _map = new();

        public FileAuthenticationStore(string fileName)
        {
            _path = fileName;
            Load();
        }

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
            Commit();
        }

        public void Set(GrabberAuthenticationRequest request, string state)
        {
            Set(request.Grabber.StringId, state);
        }

        public void Delete(string grabberId)
        {
            _map.Remove(grabberId);
            Commit();
        }

        private void Load()
        {
            _map.Clear();
            if (!File.Exists(_path))
            {
                File.Create(_path).Dispose();
                return;
            }
            var lines = File.ReadAllLines(_path, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                var pair = line.Split(new[] { '=' }, 2);
                _map[pair[0]] = pair[1];
            }
        }

        private void Commit()
        {
            var lines = _map
                .Select(p => $"{p.Key}={p.Value}");
            File.WriteAllLines(_path, lines);
        }
    }
}
