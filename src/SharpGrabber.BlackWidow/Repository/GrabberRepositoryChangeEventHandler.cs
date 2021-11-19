using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Represents a method that handles changes of a grabber repository.
    /// </summary>
    /// <param name="repository">The updated repository</param>
    /// <param name="feed">The new feed</param>
    /// <param name="previousFeed">The previous feed</param>
    public delegate void GrabberRepositoryChangeEventHandler(IGrabberRepository repository, IGrabberRepositoryFeed feed, IGrabberRepositoryFeed previousFeed);
}
