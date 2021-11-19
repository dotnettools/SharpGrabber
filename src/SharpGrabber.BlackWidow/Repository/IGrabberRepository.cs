using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Represents a BlackWidow repository.
    /// </summary>
    public interface IGrabberRepository : IDisposable
    {
        /// <summary>
        /// Gets whether or not this repository supports putting scripts.
        /// </summary>
        bool CanPut { get; }

        /// <summary>
        /// Gets whether or not this implementation supports notifying changes.
        /// </summary>
        bool CanNotifyChanges { get; }

        /// <summary>
        /// Gets the latest feed from the source.
        /// This will result in an I/O operation such as a web service call, disk scan etc.
        /// </summary>
        Task<IGrabberRepositoryFeed> GetFeedAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Fetches the source of a specific script.
        /// </summary>
        Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript script, CancellationToken cancellationToken);

        /// <summary>
        /// Puts the <paramref name="script"/> with its <paramref name="source"/> into the repository.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if putting scripts into this repository is not supported.</exception>
        Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to subscribe to changes to the repository.
        /// </summary>
        /// <returns>The subscription, or NULL if the implementation is unable to detect its changes.</returns>
        Task<IGrabberRepositorySubscription> SubscribeAsync();
    }

    /// <summary>
    /// Defines extension methods for <see cref="IGrabberRepository"/>.
    /// </summary>
    public static class GrabberRepositoryExtensions
    {
        /// <summary>
        /// Gets the latest feed from the source.
        /// This will result in an I/O operation such as a web service call, disk scan etc.
        /// </summary>
        public static Task<IGrabberRepositoryFeed> GetFeedAsync(this IGrabberRepository repository)
        {
            return repository.GetFeedAsync(CancellationToken.None);
        }

        /// <summary>
        /// Fetches the source of a specific script.
        /// </summary>
        public static Task<IGrabberScriptSource> FetchSourceAsync(this IGrabberRepository repository, IGrabberRepositoryScript script)
        {
            return repository.FetchSourceAsync(script, CancellationToken.None);
        }

        /// <summary>
        /// Puts the <paramref name="script"/> with its <paramref name="source"/> into the repository.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if putting scripts into this repository is not supported.</exception>
        public static Task PutAsync(this IGrabberRepository repository, IGrabberRepositoryScript script, IGrabberScriptSource source)
        {
            return repository.PutAsync(script, source, CancellationToken.None);
        }
    }
}
