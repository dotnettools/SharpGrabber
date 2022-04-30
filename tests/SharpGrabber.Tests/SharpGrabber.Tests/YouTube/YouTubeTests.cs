using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.YouTube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.Tests.YouTube
{
    public class YouTubeTests
    {
        [Fact]
        public async Task GrabFromYouTube()
        {
            var uri = new Uri("https://www.youtube.com/watch?v=J42SZXS-_Qo");
            var grabber = new YouTubeGrabber(GrabberServices.Default);
            var result = await grabber.GrabAsync(uri);
            Assert.Equal("ASMR Programming - Coding a Snake Game - No Talking", result.Title);
            Assert.Contains(result.Resources, r => r is GrabbedMedia);
        }
    }
}
