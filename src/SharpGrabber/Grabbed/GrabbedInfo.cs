using System;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Contains basic information e.g. Total views, likes etc.
    /// </summary>
    /// <remarks>
    /// Each property of this class is optional/nullable, as they might not be applicable for
    /// different sources.
    /// </remarks>
    [GrabbedType("Info")]
    public class GrabbedInfo : IGrabbed
    {
        /// <summary>
        /// Author name
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Total views of the page
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// Overall duration of media if available
        /// </summary>
        public TimeSpan? Length { get; set; }
    }
}
