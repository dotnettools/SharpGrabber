using DotNetTools.SharpGrabber.BlackWidow.Internal;
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

        public override int GetHashCode()
        {
            return HashCodeUtils.Compute(_scripts.Values.ToArray());
        }

        public override bool Equals(object obj)
        {
            if (obj is GrabberRepositoryFeed feed)
            {
                if (_scripts.Count != feed._scripts.Count)
                    return false;
                foreach (var ownScript in _scripts.Values)
                {
                    var otherScript = feed.GetScript(ownScript.Id);
                    if (otherScript == null || !otherScript.Equals(ownScript))
                        return false;
                }
                return true;
            }
            return base.Equals(obj);
        }
    }
}
