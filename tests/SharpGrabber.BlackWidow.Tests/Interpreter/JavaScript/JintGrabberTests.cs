﻿using DotNetTools.SharpGrabber.BlackWidow.Exceptions;
using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using Jint;
using Jint.Native;
using SharpGrabber.BlackWidow.Tests.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using DotNetTools.SharpGrabber.Exceptions;

namespace SharpGrabber.BlackWidow.Tests.Interpreter.JavaScript
{
    public class JintGrabberTests
    {
        [Theory]
        [InlineData(@"return")]
        [InlineData(@"grabber.supports=function () {}")]
        [InlineData(@"grabber.grab=function () {}")]
        [InlineData(@"grabber.supports=function () {};grabber.grab=function () {}", false)]
        public async Task Test_MethodsGrabAndSupportsMustBeDefined(string src, bool expectThrow = true)
        {
            async Task TryLoad()
            {
                await LoadScript(src);
            }

            if (expectThrow)
                await Assert.ThrowsAsync<ScriptInterpretException>(TryLoad);
            else
                await TryLoad();
        }

        [Theory]
        [InlineData(true, @"
            grabber.supports=()=>{};
            grabber.grab=(request, result, resolve, reject) => {
                resolve();
            };
        ")]
        [InlineData(false, @"
            grabber.supports=()=>{};
            grabber.grab=(request, result, resolve, reject) => {
                reject();
            };
        ")]
        [InlineData(null, @"
            grabber.supports=()=>{};
            grabber.grab=(request, result, resolve, reject) => {};
        ")]
        public async Task Test_GrabResolveAndReject(bool? expectedSuccess, string src)
        {
            var html = AssetsAccessor.GetText("Html/SamplePage.html");
            var grabber = await LoadScript(src, new
            {
                pageHtml = html,
            });

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1));

            Task GrabAsync()
                => grabber.GrabAsync(new Uri("https://example.site"), cancellationTokenSource.Token);

            switch (expectedSuccess)
            {
                case null:
                    // expect ethernal wait
                    await Assert.ThrowsAsync<TaskCanceledException>(GrabAsync);
                    break;

                case false:
                    await Assert.ThrowsAsync<GrabException>(GrabAsync);
                    break;

                case true:
                    await GrabAsync();
                    break;
            }
        }

        [Theory]
        [InlineData("BLACKWIDOW", @"
            grabber.supports=()=>{};
            grabber.grab=(request, result, resolve, reject) => {
                const doc = html.parse(data.pageHtml);
                const elem = doc.select('head title');
                data.deliver(elem.innerText);
                resolve();
            };
        ")]
        public async Task Test_HtmlSelector(object expectedData, string src)
        {
            var html = AssetsAccessor.GetText("Html/SamplePage.html");
            var grabber = await LoadScript(src, new
            {
                pageHtml = html,
                deliver = (Action<object>)(data => Assert.Equal(expectedData, data))
            });
            await grabber.GrabAsync(new Uri("https://example.site"));
        }

        private static async Task<IGrabber> LoadScript(string source, object dataObject = null)
        {
            var collection = new GrabbedTypeCollection();
            var apiService = new DefaultInterpreterApiService(GrabberServices.Default, collection, ApiTypeConverter.Default);
            var host = new ScriptHost();
            host.OnAlert += o =>
            {
                Debugger.Break();
            };
            host.OnLog += log =>
            {
                Debugger.Break();
            };
            var i = new JintJavaScriptInterpreter(apiService, GrabberServices.Default, host);
            var script = new GrabberRepositoryScript();
            var src = new GrabberScriptSource(source);
            var options = new GrabberScriptInterpretOptions
            {
                ExposedData = dataObject == null ? null : new Dictionary<string, object>
                {
                    { "data", dataObject }
                }
            };
            var grabber = await i.InterpretAsync(script, src, 1, options);
            return grabber;
        }
    }
}