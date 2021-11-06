using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using DotNetTools.SharpGrabber.BlackWidow.Repository.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests
{
    public class BlackWidowServiceTests
    {
        private const string DummySource = @"
            grabber.supports = function() {
                return false
            }
            grabber.grab = function (a, b, resolve, reject) {
                reject()
                return false
            }
";

        private IGrabberScriptInterpreterService _interpreterService;
        private IGrabberRepository _localRepository;
        private IGrabberRepository _remoteRepository;
        private IBlackWidowService _service;
        private IInterpreterApiService _apiService;
        private ScriptHost _scriptHost;
        private GrabbedTypeCollection _typeCollection;
        private IApiTypeConverter _typeConverter;

        [Fact]
        public async Task Test_CorrectRepositoryReferences()
        {
            await Initialize();
            Assert.Equal(_localRepository, _service.LocalRepository);
            Assert.Equal(_remoteRepository, _service.RemoteRepository);
        }

        [Fact]
        public async Task Test_UpdateFeed()
        {
            await Initialize();
            await _service.UpdateFeedAsync();
            var grabber = await _service.GetScriptAsync("A");
            Assert.Null(grabber);

            var script = new GrabberRepositoryScript
            {
                Id = "A",
                Type = GrabberScriptType.JavaScript,
                SupportedRegularExpressions = new[] { ".*" },
            };
            await _remoteRepository.PutAsync(script, new GrabberScriptSource(DummySource));

            await _service.UpdateFeedAsync();
            grabber = await _service.GetScriptAsync("A");
            Assert.NotNull(grabber);
        }

        private async Task Initialize()
        {
            _localRepository = new InMemoryRepository();
            _remoteRepository = new InMemoryRepository();
            _scriptHost = new ScriptHost();
            _typeCollection = new GrabbedTypeCollection();
            _typeConverter = ApiTypeConverter.Default;
            _apiService = new DefaultInterpreterApiService(GrabberServices.Default, _typeCollection, _typeConverter);
            _interpreterService = new GrabberScriptInterpreterService();
            _interpreterService.RegisterJint(_apiService, GrabberServices.Default, _scriptHost);
            _service = await BlackWidowService.CreateAsync(_localRepository, _remoteRepository, _scriptHost, _interpreterService);
        }
    }
}
