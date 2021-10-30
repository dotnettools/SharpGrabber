using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Definitions
{
    /// <summary>
    /// Default implementation for <see cref="IGrabberScriptSource"/>
    /// </summary>
    public class GrabberScriptSource : IGrabberScriptSource
    {
        public GrabberScriptSource() { }

        public GrabberScriptSource(string source)
        {
            Source = source;
        }

        /// <summary>
        /// Gets or sets the source code of the grabber script.
        /// </summary>
        public string Source { get; set; }
    }
}
