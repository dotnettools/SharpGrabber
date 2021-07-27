using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Represents an <see cref="IGrabbed"/> that refers to a resource.
    /// </summary>
    public interface IGrabbedResource : IGrabbed
    {
        /// <summary>
        /// Gets the resource URI.
        /// </summary>
        Uri ResourceUri { get; }
    }
}
