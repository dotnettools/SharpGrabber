using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Manages a local grabber repository and keeps it constantly up-to-date.
    /// </summary>
    public interface IBlackWidowService : IDisposable
    {
        /// <summary>
        /// Gets the dynamic grabber that wraps the internal grabbers of the BlackWidow service.
        /// </summary>
        IBlackWidowGrabber Grabber { get; }

        /// <summary>
        /// Gets the script host.
        /// </summary>
        IScriptHost ScriptHost { get; }

        /// <summary>
        /// Gets the local grabber repository.
        /// </summary>
        IGrabberRepository LocalRepository { get; }

        /// <summary>
        /// Gets the remote grabber repository.
        /// </summary>
        IGrabberRepository RemoteRepository { get; }

        /// <summary>
        /// Updates feed from the remote repository.
        /// </summary>
        Task UpdateFeedAsync();

        /// <summary>
        /// Enumerates local grabbers that might support grabbing from <paramref name="uri"/>.
        /// </summary>
        IEnumerable<IGrabber> GetLocalCandidates(Uri uri);

        /// <summary>
        /// Tries to find a local grabber with <paramref name="scriptId"/>.
        /// </summary>
        IGrabber GetLocalScript(string scriptId);

        /// <summary>
        /// Gets a list of candidate grabber scripts for <paramref name="uri"/> on the remote repository.
        /// </summary>
        IEnumerable<IGrabberRepositoryScript> GetRemoteCandidates(Uri uri);

        /// <summary>
        /// Gets the grabber associated with the script if the latest version of the script with the specified <paramref name="scriptId"/>
        /// is available locally.
        /// Otherwise, it updates the local repository to contain the latest version of the script.
        /// </summary>
        Task<IGrabber> GetScriptAsync(string scriptId);
    }
}
