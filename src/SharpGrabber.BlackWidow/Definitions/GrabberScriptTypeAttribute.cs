using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GrabberScriptTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets the default value.
        /// </summary>
        public static GrabberScriptTypeAttribute Default => new();

        /// <summary>
        /// Gets or sets the file extension associated with this script type.
        /// </summary>
        public string FileExtension { get; set; }
    }
}
