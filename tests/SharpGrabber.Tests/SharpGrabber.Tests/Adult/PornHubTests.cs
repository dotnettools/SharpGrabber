using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Adult;
using DotNetTools.SharpGrabber.Grabbed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.Tests.Adult
{
    public class PornHubTests
    {
        [Fact]
        public async Task GrabFromPornHub()
        {
            var uri = new Uri("https://www.pornhub.com/view_video.php?viewkey=ph6232156154536");
            var grabber = new PornHubGrabber(GrabberServices.Default);
            var result = await grabber.GrabAsync(uri);
            Assert.Contains(result.Resources, r => r is GrabbedHlsStreamReference || r is GrabbedMedia);
        }
    }
}
