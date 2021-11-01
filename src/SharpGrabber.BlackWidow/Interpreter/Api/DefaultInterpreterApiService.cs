using DotNetTools.SharpGrabber.BlackWidow.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    public class DefaultInterpreterApiService : IInterpreterApiService
    {
        public object GetHostObject(int apiVersion, IGrabberServices grabberServices)
        {
            if (apiVersion <= 0)
                throw new ArgumentOutOfRangeException(nameof(apiVersion));
            return apiVersion switch
            {
                1 => new v1.HostObject(grabberServices),
                _ => throw new ScriptApiVersionMismatchException($"This script requires API version {apiVersion}; which is not supported."),
            };
        }

        public ProcessedGrabScript ProcessResult(int apiVersion, object hostObject)
        {
            if (apiVersion <= 0)
                throw new ArgumentOutOfRangeException(nameof(apiVersion));
            return apiVersion switch
            {
                1 => ProcessV1((v1.HostObject)hostObject),
                _ => throw new ScriptApiVersionMismatchException($"This script requires API version {apiVersion}; which is not supported."),
            };
        }

        private ProcessedGrabScript ProcessV1(v1.HostObject hostObject)
        {
            if (hostObject.Grabber.Supports == null)
                throw new ScriptInterpretException($"The {nameof(hostObject.Grabber.Supports)} function is not set.");
            if (hostObject.Grabber.Grab == null)
                throw new ScriptInterpretException($"The {nameof(hostObject.Grabber.Grab)} function is not set.");

            SupportsDelegate supports = uri =>
            {
                return hostObject.Grabber.Supports(uri?.ToString());
            };
            GrabDelegate grab = async (uri, cancellationToken, options, progress) =>
            {
                var request = new v1.GrabRequest(uri, cancellationToken, options, progress);
                var response = await hostObject.Grabber.Grab(request).ConfigureAwait(false);
                return null;
            };
            return new ProcessedGrabScript(supports, grab);
        }
    }
}
