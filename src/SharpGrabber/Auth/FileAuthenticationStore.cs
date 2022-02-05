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

        public string Get(string key, string @default = null)
        {
            return _map.TryGetValue(key, out var value) ? value : @default;
        }

        public void Set(string key, string value)
        {
            if (key.Contains('='))
                throw new NotSupportedException("The key cannot contain equal sign (=).");
            _map[key] = value;
            Commit();
        }

        public void Delete(string key)
        {
            _map.Remove(key);
            Commit();
        }

        private void Load()
        {
            _map.Clear();
            if (!File.Exists(_path))
                return;
            var lines = File.ReadAllLines(_path, Encoding.UTF8);
            foreach (var line in lines)
            {
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
