using System.Collections.Generic;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Stores all grabbed comments from the source.
    /// </summary>
    public class GrabbedComments : IGrabbed
    {
        public IReadOnlyList<GrabbedComment> Comments { get; }
    }
}
