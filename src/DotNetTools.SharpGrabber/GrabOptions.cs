using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Flags for grab options
    /// </summary>
    [Flags]
    public enum GrabOptionFlag
    {
        /// <summary>
        /// No grab flags
        /// </summary>
        None = 0,

        /// <summary>
        /// Grabber may decipher URIs automatically where necessary.
        /// </summary>
        Decipher = 1,

        /// <summary>
        /// Grabber may grab related images.
        /// </summary>
        GrabImages = 2,
    }

    /// <summary>
    /// Describes options for a grab request.
    /// </summary>
    public class GrabOptions
    {
        #region Properties
        /// <summary>
        /// Option flags
        /// </summary>
        public GrabOptionFlag Flags { get; set; } = GrabOptionFlag.None;
        #endregion

        #region Constructors
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GrabOptions()
        {
        }

        public GrabOptions(GrabOptionFlag flag) : this()
        {
            Flags = flag;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion
    }
}