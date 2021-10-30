using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Describes the state of a repository.
    /// </summary>
    public interface IGrabberRepositoryFeed
    {
        /// <summary>
        /// Tries to find a script with the specified identifier in the repository.
        /// </summary>
        /// <returns>The script if found; otherwise NULL.</returns>
        IGrabberRepositoryScript GetScript(string scriptId);

        /// <summary>
        /// Enumerates descriptors of all scripts in the repository.
        /// </summary>
        IEnumerable<IGrabberRepositoryScript> GetScripts();
    }
}
