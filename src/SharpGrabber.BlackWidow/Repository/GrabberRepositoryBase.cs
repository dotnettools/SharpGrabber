using DotNetTools.SharpGrabber.BlackWidow.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Base class for implementing <see cref="IGrabberRepository"/>
    /// </summary>
    public abstract class GrabberRepositoryBase : IGrabberRepository
    {
        private readonly object _monitoringLock = new();
        private readonly ConcurrentHashSet<IGrabberRepositorySubscription> _subscriptions = new();
        private bool _disposed;
        private bool _monitoring;

        public virtual bool CanPut => false;

        public virtual bool CanNotifyChanges => false;

        protected bool AnySubscribers => _subscriptions.Count > 0;

        public abstract Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript script);

        public abstract Task<IGrabberRepositoryFeed> GetFeedAsync();

        public virtual Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source)
        {
            throw new NotSupportedException($"Putting is not supported by {GetType()}.");
        }

        public Task<IGrabberRepositorySubscription> SubscribeAsync()
        {
            IGrabberRepositorySubscription result = null;
            if (CanNotifyChanges)
            {
                result = new Subscription(this);
                StartOrStopMonitoring();
            }
            return Task.FromResult(result);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            Dispose(true);
            _subscriptions.Clear();
            StartOrStopMonitoring();
        }

        protected void NotifyChanged(IGrabberRepositoryFeed feed)
        {
            foreach (var subscription in _subscriptions.OfType<Subscription>())
                subscription.Notify(feed);
        }

        /// <summary>
        /// Starts monitoring for changes to the repository and notifies with .
        /// </summary>
        protected virtual Task StartMonitoringAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops
        /// </summary>
        protected virtual Task StopMonitoringAsync()
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private void Unsubscribe(IGrabberRepositorySubscription subscription)
        {
            _subscriptions.Remove(subscription);
            StartOrStopMonitoring();
        }

        private void StartOrStopMonitoring()
        {
            if (_monitoring == AnySubscribers)
                return;

            lock (_monitoringLock)
            {
                if (_monitoring == AnySubscribers)
                    return;
                _monitoring = AnySubscribers;

                if (_monitoring)
                    _ = StartMonitoringAsync();
                else
                    _ = StopMonitoringAsync();
            }
        }

        private sealed class Subscription : IGrabberRepositorySubscription
        {
            private readonly GrabberRepositoryBase _base;

            public event Action<IGrabberRepositoryFeed, IGrabberRepository> FeedUpdated;

            public Subscription(GrabberRepositoryBase @base)
            {
                _base = @base;
            }

            public void Notify(IGrabberRepositoryFeed feed)
            {
                FeedUpdated?.Invoke(feed, _base);
            }

            public void Dispose()
            {
                _base.Unsubscribe(this);
            }
        }
    }
}
