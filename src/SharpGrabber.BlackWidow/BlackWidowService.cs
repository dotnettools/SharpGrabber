using DotNetTools.SharpGrabber.BlackWidow.Exceptions;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;
using DotNetTools.SharpGrabber.BlackWidow.Internal;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Default implementation for <see cref="IBlackWidowService"/>
    /// </summary>
    public class BlackWidowService : IBlackWidowService
    {
        private readonly ConcurrentDictionary<string, IGrabber> _grabbers =
            new(StringComparer.InvariantCultureIgnoreCase);
        private readonly BlackWidowGrabber _grabber;

        private readonly IGrabberRepositoryChangeDetector _changeDetector;
        private readonly ConcurrentHashSet<string> _scriptsUsed = new();
        private readonly ConcurrentHashSet<string> _scriptsUpdating = new();
        private IGrabberRepositoryFeed _localFeed;
        private IGrabberRepositoryFeed _remoteFeed;

        protected BlackWidowService(IGrabberRepository localRepository, IGrabberRepository remoteRepository,
            IGrabberServices grabberServices,
            IScriptHost scriptHost, IGrabberScriptInterpreterService interpreterService, IGrabberRepositoryChangeDetector changeDetector)
        {
            _changeDetector = changeDetector;
            Interpreters = interpreterService ?? throw new ArgumentNullException(nameof(interpreterService));
            LocalRepository = localRepository ?? throw new ArgumentNullException(nameof(localRepository));
            RemoteRepository = remoteRepository ?? throw new ArgumentNullException(nameof(remoteRepository));
            ScriptHost = scriptHost;
            changeDetector.RepositoryChanged += ChangeDetector_RepositoryChanged;
            _grabber = new BlackWidowGrabber(this, grabberServices ?? throw new ArgumentNullException(nameof(grabberServices)));
        }

        public IScriptHost ScriptHost { get; }

        /// <summary>
        /// Gets the interpreter service.
        /// </summary>
        public IGrabberScriptInterpreterService Interpreters { get; }

        public IGrabberRepository LocalRepository { get; }

        public IGrabberRepository RemoteRepository { get; }

        public IBlackWidowGrabber Grabber => _grabber;

        /// <summary>
        /// Creates a new instance of <see cref="BlackWidowService"/>.
        /// </summary>
        public static async Task<BlackWidowService> CreateAsync(IGrabberRepository localRepository,
            IGrabberRepository remoteRepository,
            IGrabberServices grabberServices,
            IScriptHost scriptHost, IGrabberScriptInterpreterService interpreterService = null,
            IGrabberRepositoryChangeDetector changeDetector = null)
        {
            interpreterService ??= new GrabberScriptInterpreterService();
            changeDetector ??= new GrabberRepositoryChangeDetector(new[] { localRepository, remoteRepository });
            var service = new BlackWidowService(localRepository, remoteRepository, grabberServices, scriptHost, interpreterService, changeDetector);
            await service.LoadLocalFeedAsync().ConfigureAwait(false);
            return service;
        }

        public IEnumerable<IGrabber> GetLocalCandidates(Uri uri)
        {
            return _grabbers.Values.Where(g => g.Supports(uri));
        }

        public IGrabber GetLocalScript(string scriptId)
            => _grabbers.GetOrDefault(scriptId);

        public IEnumerable<IGrabberRepositoryScript> GetRemoteCandidates(Uri uri)
        {
            if (_remoteFeed == null)
                return Enumerable.Empty<IGrabberRepositoryScript>();

            return _remoteFeed
                .GetScripts()
                .Where(s => s.IsMatch(uri));
        }

        public async Task<IGrabber> GetScriptAsync(string scriptId)
        {
            // init
            var localInfo = _localFeed.GetScript(scriptId);
            var remoteInfo = _remoteFeed?.GetScript(scriptId);
            if (localInfo == null)
            {
                await LoadLocalFeedAsync().ConfigureAwait(false);
                localInfo = _localFeed.GetScript(scriptId);
            }

            var updateNeeded = localInfo == null ||
                               (remoteInfo != null && remoteInfo.GetVersion() > localInfo.GetVersion());

            if (localInfo == null && remoteInfo == null)
                return null;

            // fetch the script
            if (updateNeeded)
            {
                var source = await RemoteRepository.FetchSourceAsync(remoteInfo).ConfigureAwait(false);
                await LocalRepository.PutAsync(remoteInfo, source).ConfigureAwait(false);
                _grabbers.TryRemove(scriptId, out _);
                await LoadLocalFeedAsync().ConfigureAwait(false);
            }

            // get local grabber
            return _grabbers.GetOrDefault(scriptId);
        }

        public async Task UpdateFeedAsync()
        {
            _remoteFeed = await RemoteRepository.GetFeedAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            _changeDetector?.Dispose();
        }

        private async Task LoadLocalFeedAsync()
        {
            _localFeed = await LocalRepository.GetFeedAsync().ConfigureAwait(false);
            await LoadLocalGrabbers().ConfigureAwait(false);
        }

        private async Task LoadLocalGrabbers()
        {
            foreach (var scriptInfo in _localFeed.GetScripts())
            {
                if (_grabbers.ContainsKey(scriptInfo.Id))
                    continue;
                var scriptSource = await LocalRepository.FetchSourceAsync(scriptInfo).ConfigureAwait(false);
                await LoadGrabberAsync(scriptInfo, scriptSource).ConfigureAwait(false);
            }
        }

        private async Task<IGrabber> LoadGrabberAsync(IGrabberRepositoryScript scriptInfo,
            IGrabberScriptSource scriptSource)
        {
            var interpreter = Interpreters.GetInterpreter(scriptInfo.Type);
            if (interpreter == null)
                throw new ScriptInterpretException($"No interpreter is registered for {scriptInfo.Type}.");

            var grabber = await interpreter.InterpretAsync(scriptInfo, scriptSource, scriptInfo.ApiVersion)
                .ConfigureAwait(false);
            _grabbers.TryAdd(scriptInfo.Id, grabber);
            return grabber;
        }

        private void ChangeDetector_RepositoryChanged(IGrabberRepository repository, IGrabberRepositoryFeed feed, IGrabberRepositoryFeed prevFeed)
        {
            if (repository != LocalRepository && repository != RemoteRepository)
                return;

            var isLocal = LocalRepository == repository;
            if (isLocal)
                _localFeed = feed;
            else
                _remoteFeed = feed;
            _ = UpdateGrabbersAsync(_scriptsUsed);
        }

        private async Task<bool> UpdateGrabbersAsync(IEnumerable<string> ids)
        {
            if (_remoteFeed == null)
                await UpdateFeedAsync().ConfigureAwait(false);

            var localFeed = _localFeed;
            var remoteFeed = _remoteFeed;
            if (localFeed == null || remoteFeed == null)
                return false;

            var idSet = new HashSet<string>(ids, StringComparer.InvariantCultureIgnoreCase);

            var localScripts = localFeed.GetScripts()
                .Where(s => idSet.Contains(s.Id))
                .ToDictionary(s => s.Id);
            var remoteScripts = remoteFeed.GetScripts()
                .Where(s => idSet.Contains(s.Id));

            // compare scripts
            var updateTasks = new List<Task<bool>>();
            foreach (var remoteScript in remoteScripts)
            {
                var localScript = localScripts[remoteScript.Id];
                if (localScript != null && remoteScript.GetVersion() <= localScript.GetVersion())
                    continue;
                var task = UpdateGrabberAsync(remoteScript.Id);
                updateTasks.Add(task);
            }
            await Task.WhenAll(updateTasks).ConfigureAwait(false);
            var anyUpdates = updateTasks.Any(t => t.Result);

            if (anyUpdates)
            {
                await LoadLocalGrabbers().ConfigureAwait(false);
            }
            return anyUpdates;
        }

        private async Task<bool> UpdateGrabberAsync(string id)
        {
            // get current records
            var localScript = _localFeed?.GetScript(id);
            var remoteScript = _remoteFeed?.GetScript(id);
            if (remoteScript == null)
                return false;
            if (localScript != null && localScript.GetVersion() >= remoteScript.GetVersion())
                return false;

            if (!_scriptsUpdating.Add(id))
                return false;

            try
            {
                // update script
                await GetScriptAsync(id);
            }
            finally
            {
                _scriptsUpdating.Remove(id);
            }
            return true;
        }

        private sealed class BlackWidowGrabber : GrabberBase, IBlackWidowGrabber
        {
            private readonly BlackWidowService _service;

            public BlackWidowGrabber(BlackWidowService service, IGrabberServices services) : base(services)
            {
                _service = service;
            }

            public override string StringId => "BlackWidow";

            public override string Name => "BlackWidow";

            public override GrabOptions DefaultGrabOptions { get; } = new GrabOptions(GrabOptionFlags.All);

            public IEnumerable<IGrabber> GetScriptGrabbers()
            {
                return _service._grabbers.Values.AsEnumerable();
            }

            public override bool Supports(Uri uri)
            {
                return new[] { _service._localFeed, _service._remoteFeed }
                    .Any(feed => feed?.GetScripts().Any(s => s.IsMatch(uri)) ?? false);
            }

            protected override async Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
            {
                Dictionary<string, IGrabber> GetGrabbers()
                    => _service._grabbers
                    .Where(g => g.Value.Supports(uri))
                    .ToDictionary(g => g.Key, g => g.Value);

                var grabbers = GetGrabbers();
                if (await _service.UpdateGrabbersAsync(grabbers.Keys).ConfigureAwait(false))
                {
                    grabbers = GetGrabbers();
                }

                foreach (var grabber in grabbers)
                {
                    _service._scriptsUsed.Add(grabber.Key);
                    var result = await grabber.Value.GrabAsync(uri, cancellationToken, options, progress).ConfigureAwait(false);
                    if (result != null)
                        return result;
                }
                return null;
            }
        }
    }
}