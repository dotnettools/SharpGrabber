using DotNetTools.SharpGrabber;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpGrabber.Odysee
{
    /// <summary>
    /// Defines extension methods on <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class OdyseeGrabberBuilderExtensions
    {
        /// <summary>
        /// Registers the Odysee grabber.
        /// </summary>
        public static IGrabberBuilder AddOdysee(this IGrabberBuilder builder)
            => builder.Add<OdyseeGrabber>();
    }
}
