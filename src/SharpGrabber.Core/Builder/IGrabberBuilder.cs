using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Builds a multi-purpose grabber from multiple single <see cref="IGrabber"/>s.
    /// </summary>
    public interface IGrabberBuilder
    {
        /// <summary>
        /// Includes a grabber.
        /// </summary>
        IGrabberBuilder Add(IGrabber grabber);

        /// <summary>
        /// Includes a grabber by instantiating its type with the default constructor.
        /// </summary>
        IGrabberBuilder Add<T>() where T : IGrabber;

        /// <summary>
        /// Builds the final grabber.
        /// </summary>
        IGrabber Build();
    }
}
