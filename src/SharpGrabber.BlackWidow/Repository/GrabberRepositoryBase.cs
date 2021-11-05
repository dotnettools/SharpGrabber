using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Base class for implementing <see cref="IGrabberRepository"/>
    /// </summary>
    public abstract class GrabberRepositoryBase : IGrabberRepository
    {
        private bool _disposed;

        public virtual bool CanPut => false;

        public virtual bool CanNotifyChanges => false;

        public abstract Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript script);

        public abstract Task<IGrabberRepositoryFeed> GetFeedAsync();

        public virtual Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source)
        {
            throw new NotSupportedException($"Putting is not supported by {GetType()}.");
        }

        public Task<IGrabberRepositorySubscription> SubscribeAsync()
        {
            return Task.FromResult<IGrabberRepositorySubscription>(null);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
