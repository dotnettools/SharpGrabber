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
        None = 0,

        /// <summary>
        /// Grabber may decipher URIs automatically where necessary
        /// </summary>
        Decipher = 1,
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
        public GrabOptions()
        {
        }
        
        public GrabOptions(GrabOptionFlag flag) : this()
        {
            Flags = flag;
        }
        #endregion
    }
}