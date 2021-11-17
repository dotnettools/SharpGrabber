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
        private readonly Dictionary<IGrabberRepository, IGrabberRepositoryFeed> _repositories;
        private readonly List<IDisposable> _disposables = new();
        private readonly AutoResetEvent _pollingSync = new(false);
        private IGrabberRepository[] _pollingRepositories;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _pollingIntervalCancellation;
        private TimeSpan _pollingInterval = TimeSpan.FromHours(1);

        public GrabberRepositoryChangeDetector(IEnumerable<IGrabberRepository> repositories)
        {
            _repositories = repositories.ToDictionary(r => r, r => (IGrabberRepositoryFeed)null);
            _ = ProcessRepositoriesAsync();
        }

        public event GrabberRepositoryChangeEventHandler RepositoryChanged;

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
            if (pollableOnly)
            {
                _pollingIntervalCancellation?.Cancel();
                await TriggerPollingAsync();
            }
            else
            {
                await PollAsync(_repositories.Keys, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                return;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            _pollingIntervalCancellation?.Cancel();
            _pollingIntervalCancellation = null;
            _repositories.Clear();
            RepositoryChanged = null;
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
            _pollingSync.Dispose();
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private async Task ProcessRepositoriesAsync()
        {
            var pollingRepos = new List<IGrabberRepository>();
            foreach (var repository in _repositories.Keys)
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
            if (!_pollingSync.Set())
                return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                // test if there are any repos to poll
                if (_pollingRepositories == null || _pollingRepositories.Length == 0)
                    return;

                await PollAsync(_pollingRepositories, _cancellationTokenSource.Token).ConfigureAwait(false);
            }
            finally
            {
                _pollingSync.Reset();
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

        private async Task PollAsync(IEnumerable<IGrabberRepository> repositories, CancellationToken cancellationToken)
        {
            async Task<Tuple<IGrabberRepository, IGrabberRepositoryFeed>> GetFeedAsync(IGrabberRepository repo)
            {
                var feed = await repo.GetFeedAsync(cancellationToken).ConfigureAwait(false);
                return new Tuple<IGrabberRepository, IGrabberRepositoryFeed>(repo, feed);
            };

            var tasks = new HashSet<Task<Tuple<IGrabberRepository, IGrabberRepositoryFeed>>>();
            foreach (var repo in repositories)
            {
                var task = GetFeedAsync(repo);
                tasks.Add(task);
            }

            while (tasks.Count > 0)
            {
                var task = await Task.WhenAny(tasks);
                cancellationToken.ThrowIfCancellationRequested();
                tasks.Remove(task);

                var tuple = task.Result;
                TestChanged(tuple.Item1, tuple.Item2);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private async void Subscription_FeedUpdated(IGrabberRepositoryFeed feed, IGrabberRepository repository)
        {
            if (RepositoryChanged == null)
                return;
            feed ??= await repository.GetFeedAsync().ConfigureAwait(false);
            TestChanged(repository, feed);
        }

        private void TestChanged(IGrabberRepository repository, IGrabberRepositoryFeed feed)
        {
            if (!_repositories.ContainsKey(repository))
                throw new InvalidOperationException("The repository is not registered.");

            var prevFeed = _repositories[repository];
            if (prevFeed != null && feed.Equals(prevFeed))
                // not changed
                return;

            _repositories[repository] = feed;
            RepositoryChanged?.Invoke(repository, feed, prevFeed);
        }
    }
}
