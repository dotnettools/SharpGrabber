using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    internal class InMemoryRepositoryFeed : IGrabberRepositoryFeed
    {
        private readonly ConcurrentDictionary<string, IGrabberRepositoryScript> _scripts = new();

        public InMemoryRepositoryFeed() { }

        public InMemoryRepositoryFeed(IEnumerable<IGrabberRepositoryScript> scripts)
        {
            foreach (var script in scripts)
                _scripts.TryAdd(script.Id, script);
        }

        public IGrabberRepositoryScript GetScript(string scriptId)
        {
            return _scripts.GetOrDefault(scriptId);
        }

        public IEnumerable<IGrabberRepositoryScript> GetScripts()
        {
            return _scripts.Values.AsEnumerable();
        }

        public void Add(IGrabberRepositoryScript script)
        {
            _scripts.TryAdd(script.Id, script);
        }

        public void Remove(string id)
        {
            _scripts.TryRemove(id, out _);
        }
    }
}
