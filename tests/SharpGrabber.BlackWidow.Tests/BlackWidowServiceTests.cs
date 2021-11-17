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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DotNetTools.ConvertEx;
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
        private ITypeConverter _typeConverter;

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

        [Fact]
        public async Task Test_Temp()
        {
            await Initialize();
            var script = new GrabberRepositoryScript()
            {
                Id = "temp",
                Type = GrabberScriptType.JavaScript,
            };
            var src = await System.IO.File.ReadAllTextAsync(
                @"D:\Projects\C#\SharpGrabber\blackwidow\repo\scripts\vimeo.js");
            var source = new GrabberScriptSource(src);
            await _service.LocalRepository.PutAsync(script, source);
            var grabber = await _service.GetScriptAsync("temp");
            var result = await grabber.GrabAsync(new Uri(@"https://vimeo.com/423222165"));
        }

        private async Task Initialize()
        {
            _localRepository = new InMemoryRepository();
            _remoteRepository = new InMemoryRepository();
            _scriptHost = new ScriptHost();
            _scriptHost.OnAlert += o => Debugger.Break();
            _scriptHost.OnLog += log => Debugger.Break();
            _typeCollection = new GrabbedTypeCollection();
            _typeConverter = ConvertEx.DefaultConverter;
            _apiService = new DefaultInterpreterApiService(GrabberServices.Default, _typeCollection, _typeConverter);
            _interpreterService = new GrabberScriptInterpreterService();
            _interpreterService.RegisterJint(_apiService, GrabberServices.Default, _scriptHost);
            _service = await BlackWidowService.CreateAsync(_localRepository, _remoteRepository, GrabberServices.Default, _scriptHost,
                _interpreterService);
        }
    }
}