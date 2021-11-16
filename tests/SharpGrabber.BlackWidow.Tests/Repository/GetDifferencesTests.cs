using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using DotNetTools.SharpGrabber.BlackWidow.Repository.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Repository
{
    public class GetDifferencesTests
    {
        [Fact]
        public void Test_NoDifference()
        {
            var feed1 = new GrabberRepositoryFeed();
            var feed2 = new GrabberRepositoryFeed();

            feed1.Add(new GrabberRepositoryScript
            {
                Id = "A",
                Version = "1.0",
            });

            feed2.Add(new GrabberRepositoryScript
            {
                Id = "A",
                Version = "1.0",
            });

            var differences = feed2.GetDifferences(feed1);
            Assert.Empty(differences);
        }

        [Fact]
        public void Test_OneScriptUpdated()
        {
            var feed1 = new GrabberRepositoryFeed();
            var feed2 = new GrabberRepositoryFeed();

            feed1.Add(new GrabberRepositoryScript
            {
                Id = "A",
                Version = "1.0",
            });
            feed1.Add(new GrabberRepositoryScript
            {
                Id = "B",
                Version = "1.0",
            });

            feed2.Add(new GrabberRepositoryScript
            {
                Id = "A",
                Version = "1.0",
            });
            feed2.Add(new GrabberRepositoryScript
            {
                Id = "B",
                Version = "1.1",
            });

            var differences = feed2.GetDifferences(feed1);
            Assert.Single(differences);
            Assert.Equal(GrabberRepositoryFeedDifferenceType.ScriptChanged, differences.Single().Type);
        }

        [Fact]
        public void Test_OneScriptAddedOneRemoved()
        {
            var feed1 = new GrabberRepositoryFeed();
            var feed2 = new GrabberRepositoryFeed();

            feed1.Add(new GrabberRepositoryScript
            {
                Id = "A",
                Version = "1.0",
            });

            feed2.Add(new GrabberRepositoryScript
            {
                Id = "B",
                Version = "1.0",
            });

            var differences = feed2.GetDifferences(feed1).ToArray();
            Assert.Equal(2, differences.Length);

            Assert.Contains(differences, diff => diff.Type == GrabberRepositoryFeedDifferenceType.ScriptAdded);
            Assert.Contains(differences, diff => diff.Type == GrabberRepositoryFeedDifferenceType.ScriptRemoved);
        }
    }
}
