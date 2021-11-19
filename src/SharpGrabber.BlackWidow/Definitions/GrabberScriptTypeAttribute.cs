using System;

namespace DotNetTools.SharpGrabber.BlackWidow.Definitions
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
