using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using DotNetTools.SharpGrabber.BlackWidow.Repository.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Repository
{
    public class GitHubRepositoryTests
    {
        [Fact]
        public async Task Test_GetEmptyFeed()
        {
            using var repo = CreateOfficialRepository(m => m.FeedFileName = "emptyfeed.json");
            var feed = await repo.GetFeedAsync();
            var scripts = feed.GetScripts();
            Assert.Empty(scripts);
        }

        [Fact]
        public async Task Test_GetScriptWithSource()
        {
            using var repo = CreateOfficialRepository();
            var feed = await repo.GetFeedAsync();
            var scripts = feed.GetScripts().ToArray();
            var script = scripts.First();
            var source = await repo.FetchSourceAsync(script);
            Assert.NotEmpty(source.GetSource());
        }

        private static IGrabberRepository CreateOfficialRepository(Action<GitHubGrabberRepository> modifier = null)
        {
            var repo = new OfficialGrabberRepository
            {
                BranchName = "master",
                RepoRootPath = "tests/assets/blackwidow/repo",
            };
            modifier?.Invoke(repo);
            return repo;
        }
    }
}
