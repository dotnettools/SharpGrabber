using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Represents any grabbed object by an <see cref="IGrabber"/> or a <see cref="MultiGrabber"/>.
    /// </summary>
    public interface IGrabbed
    {
        /// <summary>
        /// Link to the resource found.
        /// </summary>
        /// <remarks>
        /// Most grabbers return <see cref="IGrabbed"/> objects that refer to exactly one download link.
        /// If a grabber does not refer to any URI or refers to multiple URIs, this property should be null.
        /// </remarks>
        Uri Uri { get; }
    }
}
