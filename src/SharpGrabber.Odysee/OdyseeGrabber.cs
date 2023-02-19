using DotNetTools.SharpGrabber;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SharpGrabber.Odysee
{
    /// <summary>
    /// Implements <see cref="IGrabber"/> for the Odysee platform.
    /// </summary>
    public sealed class OdyseeGrabber : GrabberBase
    {
        private static readonly Regex UriRegex = new(
            @"^https?://(www\.)?odysee\.com/@([\w:]+)/([^\/]+)/?.*$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public OdyseeGrabber(IGrabberServices services) : base(services)
        {
        }

        public override string StringId => "odysee.com";

        public override string Name => "Odysee";

        public override bool Supports(Uri uri)
        {
            return UriRegex.IsMatch(uri.ToString());
        }

        protected override Task<GrabResult> InternalGrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress)
        {
            throw new NotImplementedException();
        }
    }
}
