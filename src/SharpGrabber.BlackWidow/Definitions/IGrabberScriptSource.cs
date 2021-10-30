using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Provides access to the source of a grabber script.
    /// </summary>
    public interface IGrabberScriptSource
    {
        /// <summary>
        /// Gets the source code of the grabber script.
        /// </summary>
        string Source { get; }
    }
}
