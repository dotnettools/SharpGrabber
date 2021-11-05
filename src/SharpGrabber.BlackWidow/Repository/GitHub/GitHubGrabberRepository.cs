using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository.GitHub
{
    /// <summary>
    /// Defines a grabber repository that fetches .
    /// </summary>
    public class GitHubGrabberRepository : IGrabberRepository
    {
        public bool CanPut => false;

        /// <summary>
        /// Gets or sets the name of the repository e.g. 'dotnettools/SharpGrabber'
        /// </summary>
        public string Repository { get; set; } = "dotnettools/SharpGrabber";

        /// <summary>
        /// Gets or sets the branch name.
        /// </summary>
        public string BranchName { get; set; } = "master";

        /// <summary>
        /// Gets or sets the path to the directory that contains the feed file and the scripts.
        /// </summary>
        public string ScriptsRootPath { get; set; } = "blackwidow/scripts";

        /// <summary>
        /// Gets or sets the name of the feed file.
        /// </summary>
        public string FeedFileName { get; set; } = "feed.json";

        public Task<IGrabberRepositoryFeed> GetFeedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript script)
        {
            throw new NotImplementedException();
        }

        public Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source)
        {
            throw new NotSupportedException("Putting is not supported on GitHub repositories.");
        }
    }
}
