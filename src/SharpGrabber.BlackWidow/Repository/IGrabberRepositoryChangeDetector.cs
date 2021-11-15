using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Detects changes in a grabber repository.
    /// </summary>
    public interface IGrabberRepositoryChangeDetector : IDisposable
    {
        /// <summary>
        /// Invoked when a change in a repository gets detected.
        /// </summary>
        event GrabberRepositoryChangeEventHandler RepositoryChanged;

        /// <summary>
        /// Forces a manual update of all repository feeds.
        /// </summary>
        /// <param name="pollableOnly">Whether or not to only update feeds of repositories that don't support change notification.</param>
        Task ForceUpdateFeedAsync(bool pollableOnly = true);
    }
}
