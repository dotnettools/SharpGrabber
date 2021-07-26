using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Flags for grab options
    /// </summary>
    [Flags]
    public enum GrabOptionFlags
    {
        None = 0,

        /// <summary>
        /// Grabber may decipher URIs automatically where necessary.
        /// </summary>
        Decipher = 1,

        /// <summary>
        /// Grabber may grab related images.
        /// </summary>
        GrabImages = 2,

        /// <summary>
        /// Grabber may decrypt the output stream via <see cref="GrabResult.OutputStreamWrapper"/>.
        /// </summary>
        Decrypt = 4,
    }

    /// <summary>
    /// Describes options for a grab request.
    /// </summary>
    public class GrabOptions
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GrabOptions()
        {
        }

        public GrabOptions(GrabOptionFlags flag) : this()
        {
            Flags = flag;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Option flags
        /// </summary>
        public GrabOptionFlags Flags { get; set; } = GrabOptionFlags.None;
    }
}