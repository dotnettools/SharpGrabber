using DotNetTools.SharpGrabber.BlackWidow.Repository.GitHub;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Defines the official SharpGrabber repository.
    /// </summary>
    public class OfficialGrabberRepository : GitHubGrabberRepository
    {
        public OfficialGrabberRepository()
        {
            Setup();
        }

        public OfficialGrabberRepository(HttpClient httpClient, bool ownClient = true) : base(httpClient, ownClient)
        {
            Setup();
        }

        private void Setup()
        {
            Repository = BlackWidowConstants.GitHub.OfficialRepository.RepositoryAddress;
            BranchName = BlackWidowConstants.GitHub.OfficialRepository.MasterBranch;
            RepoRootPath = BlackWidowConstants.GitHub.OfficialRepository.RootPath;
            FeedFileName = BlackWidowConstants.GitHub.OfficialRepository.FeedFileName;
        }
    }
}
