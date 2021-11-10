using DotNetTools.SharpGrabber.BlackWidow.Exceptions;
using DotNetTools.SharpGrabber.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.ConvertEx;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    public class DefaultInterpreterApiService : IInterpreterApiService
    {
        private readonly IGrabberServices _grabberServices;
        private readonly IGrabbedTypeCollection _grabbedTypeCollection;
        private readonly ITypeConverter _typeConverter;

        public DefaultInterpreterApiService(IGrabberServices grabberServices,
            IGrabbedTypeCollection grabbedTypeCollection,
            ITypeConverter typeConverter)
        {
            _grabberServices = grabberServices;
            _grabbedTypeCollection = grabbedTypeCollection;
            _typeConverter = typeConverter;
        }

        public object GetHostObject(int apiVersion, IGrabberServices grabberServices)
        {
            if (apiVersion <= 0)
                throw new ArgumentOutOfRangeException(nameof(apiVersion));
            return apiVersion switch
            {
                1 => new v1.HostObject(grabberServices),
                _ => throw new ScriptApiVersionMismatchException(
                    $"This script requires API version {apiVersion}; which is not supported."),
            };
        }

        public ProcessedGrabScript ProcessResult(int apiVersion, object hostObject)
        {
            if (apiVersion <= 0)
                throw new ArgumentOutOfRangeException(nameof(apiVersion));
            return apiVersion switch
            {
                1 => ProcessV1((v1.HostObject) hostObject),
                _ => throw new ScriptApiVersionMismatchException(
                    $"This script requires API version {apiVersion}; which is not supported."),
            };
        }

        private ProcessedGrabScript ProcessV1(v1.HostObject hostObject)
        {
            if (hostObject.Grabber.Supports == null)
                throw new ScriptInterpretException($"The {nameof(hostObject.Grabber.Supports)} function is not set.");
            if (hostObject.Grabber.Grab == null)
                throw new ScriptInterpretException($"The {nameof(hostObject.Grabber.Grab)} function is not set.");

            bool supports(Uri uri)
            {
                return hostObject.Grabber.Supports(uri?.ToString());
            }

            async Task<GrabResult> grab(Uri uri, CancellationToken cancellationToken, GrabOptions options,
                IProgress<double> progress)
            {
                var grabbedList = new List<IGrabbed>();
                var result = new GrabResult(uri, grabbedList);

                var request = new v1.GrabRequest(uri, cancellationToken, options, progress);
                var response = new v1.GrabResponse(result, grabbedList, _grabberServices, _grabbedTypeCollection,
                    _typeConverter);

                var promise = new TaskCompletionSource<bool>();
                cancellationToken.Register(promise.SetCanceled);
                void resolve() => promise.SetResult(true);

                void reject()
                {
                    promise.SetResult(false);
                    throw new GrabException();
                }

                hostObject.Grabber.Grab(request, response, resolve, reject);
                await promise.Task.ConfigureAwait(false);

                return result;
            }

            return new ProcessedGrabScript(supports, grab);
        }
    }
}