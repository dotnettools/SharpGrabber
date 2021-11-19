using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Describes a subscription to changes of a grabber repository.
    /// </summary>
    /// <remarks>
    /// To unsubscribe, the instance should be disposed.
    /// </remarks>
    public interface IGrabberRepositorySubscription : IDisposable
    {
        /// <summary>
        /// Invoked when the feed gets updated.
        /// </summary>
        event Action<IGrabberRepositoryFeed, IGrabberRepository> FeedUpdated;
    }
}
