using System.Collections.Generic;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Stores all grabbed comments from the source.
    /// </summary>
    [GrabbedType("Comments")]
    public class GrabbedComments : IGrabbed
    {
        /// <summary>
        /// Gets the list of comments.
        /// </summary>
        public IReadOnlyCollection<GrabbedComment> Comments { get; set; }
    }
}
