using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository.Memory
{
    /// <summary>
    /// In-memory implementation of grabber repository.
    /// </summary>
    public class InMemoryRepository : IGrabberRepository
    {
        private readonly Dictionary<string, ScriptInfo> _scripts = new();

        public Task<IGrabberScriptSource> FetchScriptAsync(IGrabberRepositoryScript script)
        {
            var info = _scripts.GetOrDefault(script.Id);
            return Task.FromResult(info?.Source);
        }

        public Task<IGrabberRepositoryFeed> GetFeedAsync()
        {
            var feed = new GrabberRepositoryFeed(_scripts.Values.Select(i => i.Script));
            return Task.FromResult<IGrabberRepositoryFeed>(feed);
        }

        public Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source)
        {
            var info = new ScriptInfo(script, source);
            _scripts[script.Id] = info;
            return Task.CompletedTask;
        }

        private sealed class ScriptInfo
        {
            public ScriptInfo(IGrabberRepositoryScript script, IGrabberScriptSource source)
            {
                Script = script;
                Source = source;
            }

            public IGrabberRepositoryScript Script { get; }

            public IGrabberScriptSource Source { get; }
        }
    }
}
