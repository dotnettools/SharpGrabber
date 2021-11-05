using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Default in-memory implementation for <see cref="IGrabberRepositoryFeed"/>
    /// </summary>
    public class GrabberRepositoryFeed : IGrabberRepositoryFeed
    {
        private readonly ConcurrentDictionary<string, IGrabberRepositoryScript> _scripts = new();

        public GrabberRepositoryFeed() { }

        public GrabberRepositoryFeed(IEnumerable<IGrabberRepositoryScript> scripts)
        {
            if (scripts == null)
                throw new ArgumentNullException(nameof(scripts));
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
