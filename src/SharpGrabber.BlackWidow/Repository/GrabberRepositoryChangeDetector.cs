using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Default implementation for <see cref="IGrabberRepositoryChangeDetector"/>
    /// </summary>
    public class GrabberRepositoryChangeDetector : IGrabberRepositoryChangeDetector
    {
        private readonly IGrabberRepository[] _repositories;
        private readonly List<IDisposable> _disposables = new();
        private readonly Mutex _pollingMutex = new(false);
        private IGrabberRepository[] _pollingRepositories;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _pollingIntervalCancellation;
        private TimeSpan _pollingInterval = TimeSpan.FromHours(1);

        public GrabberRepositoryChangeDetector(IEnumerable<IGrabberRepository> repositories)
        {
            _repositories = repositories.ToArray();
            _ = ProcessRepositoriesAsync();
        }

        public event Action<IGrabberRepository, IGrabberRepositoryFeed> RepositoryChanged;

        /// <summary>
        /// Gets or sets the polling interval, which is the minimum time to wait before fetching the feed of a manually trackable repository.
        /// The default value is 1h.
        /// </summary>
        public TimeSpan PollingInterval
        {
            get => _pollingInterval;
            set
            {
                _pollingInterval = value;
                _ = TriggerDelayedPollingAsync();
            }
        }

        public async Task ForceUpdateFeedAsync(bool pollableOnly = true)
        {
            _pollingIntervalCancellation?.Cancel();
            await TriggerPollingAsync();
        }

        public void Dispose()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                return;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            _pollingIntervalCancellation?.Cancel();
            _pollingIntervalCancellation = null;
            RepositoryChanged = null;
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
            _pollingMutex.Dispose();
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private async Task ProcessRepositoriesAsync()
        {
            var pollingRepos = new List<IGrabberRepository>();
            foreach (var repository in _repositories)
            {
                if (repository.CanNotifyChanges)
                {
                    var subscription = await repository.SubscribeAsync().ConfigureAwait(false);
                    subscription.FeedUpdated += Subscription_FeedUpdated;
                    _disposables.Add(subscription);
                    continue;
                }

                pollingRepos.Add(repository);
            }
            _pollingRepositories = pollingRepos.ToArray();
            _ = TriggerPollingAsync();
        }

        private async Task TriggerPollingAsync()
        {
            if (!_pollingMutex.WaitOne(TimeSpan.Zero))
                return;

            try
            {
                // test if there are any repos to poll
                if (_pollingRepositories == null || _pollingRepositories.Length == 0)
                    return;

                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            finally
            {
                _pollingMutex.ReleaseMutex();
            }

            // trigger delayed polling
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            _ = TriggerDelayedPollingAsync();
        }

        private async Task TriggerDelayedPollingAsync()
        {
            _pollingIntervalCancellation?.Cancel();
            _pollingIntervalCancellation = new();
            await Task.Delay(_pollingInterval, _pollingIntervalCancellation.Token).ConfigureAwait(false);
            await TriggerPollingAsync().ConfigureAwait(false);
        }

        private void Subscription_FeedUpdated(IGrabberRepositoryFeed feed, IGrabberRepository repository)
        {
            throw new NotImplementedException();
        }
    }
}
