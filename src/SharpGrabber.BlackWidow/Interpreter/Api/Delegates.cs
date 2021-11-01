using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api
{
    public delegate bool SupportsDelegate(Uri uri);

    public delegate Task<GrabResult> GrabDelegate(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress);
}
