using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Defines information about a class implementing <see cref="IGrabbed"/>.
    /// </summary>
    public class GrabbedTypeAttribute : Attribute
    {
        public GrabbedTypeAttribute(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the unique identifier of the type.
        /// </summary>
        public string Id { get; }
    }
}
