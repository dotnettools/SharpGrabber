using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class GrabRequest
    {
        private readonly Uri _uri;
        private readonly CancellationToken _cancellationToken;
        private readonly GrabOptions _options;
        private readonly IProgress<double> _progress;

        public GrabRequest(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            _uri = uri;
            _cancellationToken = cancellationToken;
            _options = options;
            _progress = progress;
        }
    }
}
