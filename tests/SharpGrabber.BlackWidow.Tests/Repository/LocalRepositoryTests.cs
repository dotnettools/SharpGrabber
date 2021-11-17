using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.BlackWidow.Tests.Repository
{
    public class LocalRepositoryTests
    {
        private const string DirName = "BlackWidowTestRepo";

        [Theory]
        [InlineData("A", "B", "C")]
        public async Task Test_StoreAndFetch(params string[] ids)
        {
            var dir = Path.Combine(Path.GetTempPath(), DirName);
            Directory.CreateDirectory(dir);
            try
            {
                var repo = new PhysicalGrabberRepository(dir);

                foreach (var id in ids)
                {
                    await repo.PutAsync(new GrabberRepositoryScript
                    {
                        Id = id,
                        Name = id,
                        Type = GrabberScriptType.JavaScript,
                    }, new GrabberScriptSource(id));
                }

                var feed = await repo.GetFeedAsync();
                foreach (var id in ids)
                {
                    var script = feed.GetScript(id);
                    Assert.NotNull(script);
                    Assert.Equal(id, script.Name);
                    var source = await repo.FetchSourceAsync(script);
                    Assert.Equal(id, source.GetSource());
                }
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
