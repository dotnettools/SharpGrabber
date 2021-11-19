using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Represents a BlackWidow grabber.
    /// </summary>
    public interface IBlackWidowGrabber : IGrabber
    {
        /// <summary>
        /// Enumerates internal grabbers, each representing a single grabber script.
        /// </summary>
        IEnumerable<IGrabber> GetScriptGrabbers();
    }
}
