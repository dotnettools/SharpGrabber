using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using DotNetTools.SharpGrabber.BlackWidow.Repository.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.BlackWidow.Definitions;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Repository
{
    public class ChangeDetectorTests
    {
        [Fact]
        public async Task Test_ForceUpdateFeed()
        {
            var repo = new InMemoryRepository();
            using var detector = new GrabberRepositoryChangeDetector(new[] { repo });
            await detector.ForceUpdateFeedAsync();
            IEnumerable<GrabberRepositoryFeedDifference> diffs = null;
            detector.RepositoryChanged += (repo, feed, prevFeed) =>
            {
                diffs = feed.GetDifferences(prevFeed);
            };
            await repo.PutAsync(new GrabberRepositoryScript
            {
                Id = "TEST"
            }, new GrabberScriptSource(string.Empty));
            await detector.ForceUpdateFeedAsync();
            Assert.NotNull(diffs);
            Assert.Single(diffs);
            Assert.Equal("TEST", diffs.Single().OwnScript.Id);
        }
    }
}
