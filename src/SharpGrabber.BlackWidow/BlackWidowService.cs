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
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Default implementation for <see cref="IBlackWidowService"/>
    /// </summary>
    public class BlackWidowService : IBlackWidowService
    {
        private readonly ConcurrentDictionary<string, IGrabber> _grabbers =
            new(StringComparer.InvariantCultureIgnoreCase);

        private IGrabberRepositoryFeed _localFeed;
        private IGrabberRepositoryFeed _remoteFeed;

        protected BlackWidowService(IGrabberRepository localRepository, IGrabberRepository remoteRepository,
            IScriptHost scriptHost, IGrabberScriptInterpreterService interpreterService)
        {
            Interpreters = interpreterService ?? throw new ArgumentNullException(nameof(interpreterService));
            LocalRepository = localRepository ?? throw new ArgumentNullException(nameof(localRepository));
            RemoteRepository = remoteRepository ?? throw new ArgumentNullException(nameof(remoteRepository));
            ScriptHost = scriptHost;
        }

        public IScriptHost ScriptHost { get; }

        /// <summary>
        /// Gets the interpreter service.
        /// </summary>
        public IGrabberScriptInterpreterService Interpreters { get; }

        public IGrabberRepository LocalRepository { get; }

        public IGrabberRepository RemoteRepository { get; }

        /// <summary>
        /// Creates a new instance of <see cref="BlackWidowService"/>.
        /// </summary>
        public static async Task<BlackWidowService> CreateAsync(IGrabberRepository localRepository,
            IGrabberRepository remoteRepository,
            IScriptHost scriptHost, IGrabberScriptInterpreterService interpreterService = null)
        {
            interpreterService ??= new GrabberScriptInterpreterService();
            var service = new BlackWidowService(localRepository, remoteRepository, scriptHost, interpreterService);
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
                await LoadLocalFeedAsync().ConfigureAwait(false);
            }

            // get local grabber
            return _grabbers.GetOrDefault(scriptId);
        }

        public async Task UpdateFeedAsync()
        {
            _remoteFeed = await RemoteRepository.GetFeedAsync().ConfigureAwait(false);
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
    }
}