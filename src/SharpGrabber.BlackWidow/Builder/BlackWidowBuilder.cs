using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Build a BlackWidow service.
    /// </summary>
    public sealed class BlackWidowBuilder : IBlackWidowBuilder
    {
        private IGrabberRepository _localRepository;
        private IGrabberRepository _remoteRepository;
        private IGrabberServices _grabberServices;
        private IGrabberScriptInterpreterService _interpreterService;
        private IScriptHost _scriptHost = new ScriptHost();
        private IGrabberRepositoryChangeDetector _changeDetector;

        private BlackWidowBuilder() { }

        /// <summary>
        /// Creates a new <see cref="BlackWidowBuilder"/>.
        /// </summary>
        public static BlackWidowBuilder New()
            => new();

        public async Task<IBlackWidowService> BuildAsync()
        {
            if (_localRepository == null)
                throw new InvalidOperationException("Local repository is unspecified.");
            if (_remoteRepository == null)
                throw new InvalidOperationException("Remote repository is unspecified.");
            var changeDetector = _changeDetector ?? new GrabberRepositoryChangeDetector(new[] { _localRepository, _remoteRepository });
            if (_interpreterService == null)
                SetDefaultInterpreterService();
            var grabberServices = _grabberServices ?? GrabberServices.Default;

            var service = await BlackWidowService.CreateAsync(_localRepository, _remoteRepository,
                grabberServices ?? throw new InvalidOperationException("Grabber services instance is unspecified."),
                _scriptHost ?? throw new InvalidOperationException("Script host is unspecified."),
                _interpreterService, changeDetector).ConfigureAwait(false);
            return service;
        }

        public IBlackWidowBuilder ConfigureLocalRepository(Action<IBlackWidowRepositoryConfigurator> configurator)
        {
            var cfg = new BlackWidowRepositoryConfigurator();
            configurator(cfg);
            _localRepository = cfg.Repository ?? throw new InvalidOperationException("No");
            return this;
        }

        public IBlackWidowBuilder ConfigureRemoteRepository(Action<IBlackWidowRepositoryConfigurator> configurator)
        {
            var cfg = new BlackWidowRepositoryConfigurator();
            configurator(cfg);
            _remoteRepository = cfg.Repository ?? throw new InvalidOperationException("No");
            return this;
        }

        public IBlackWidowBuilder SetChangeDetector(IGrabberRepositoryChangeDetector changeDetector)
        {
            _changeDetector = changeDetector;
            return this;
        }

        public IBlackWidowBuilder SetScriptHost(IScriptHost scriptHost)
        {
            _scriptHost = scriptHost;
            return this;
        }

        public IBlackWidowBuilder UseInterpreterService(IGrabberScriptInterpreterService interpreterService)
        {
            _interpreterService = interpreterService;
            return this;
        }

        public IBlackWidowBuilder SetGrabberServices(IGrabberServices grabberServices)
        {
            _grabberServices = grabberServices;
            return this;
        }

        public IBlackWidowBuilder ConfigureInterpreterService(Action<IGrabberScriptInterpreterServiceConfigurator> configure)
        {
            var configurator = new GrabberScriptInterpreterServiceConfigurator();
            configure(configurator);

            var interpreterService = configurator.Build();
            return UseInterpreterService(interpreterService);
        }

        private void SetDefaultInterpreterService()
        {
            var service = new GrabberScriptInterpreterService();
            ConfigureInterpreterService(cfg => cfg.AddJint());
        }
    }
}
