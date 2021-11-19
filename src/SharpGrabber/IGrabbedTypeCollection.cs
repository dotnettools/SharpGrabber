using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// When implemented, collects types implementing <see cref="IGrabbed"/> and provides access to them as needed.
    /// </summary>
    public interface IGrabbedTypeCollection
    {
        /// <summary>
        /// Gets a registered type implementing <see cref="IGrabbed"/> with identifier <paramref name="grabbedId"/>.
        /// </summary>
        Type this[string grabbedId] { get; }

        /// <summary>
        /// Gets a registered type implementing <see cref="IGrabbed"/> with identifier <paramref name="grabbedId"/>.
        /// </summary>
        Type GetGrabbed(string grabbedId);
    }
}
