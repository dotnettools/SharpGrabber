using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        /// Gets the latest feed from the source.
        /// This will result in an I/O operation such as a web service call, disk scan etc.
        /// </summary>
        Task<IGrabberRepositoryFeed> GetFeedAsync();

        /// <summary>
        /// Fetches the source of a specific script.
        /// </summary>
        Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript script);

        /// <summary>
        /// Puts the <paramref name="script"/> with its <paramref name="source"/> into the repository.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if putting scripts into this repository is not supported.</exception>
        Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source);
    }
}
