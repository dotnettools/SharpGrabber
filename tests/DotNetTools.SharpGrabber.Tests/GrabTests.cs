using DotNetTools.SharpGrabber.Internal.Grabbers;
using System;
using Xunit;

namespace DotNetTools.SharpGrabber.Tests
{
    public class GrabTests
    {
        [Fact]
        public async void Test_Instagram()
        {
            var grabber = new InstagramGrabber();
            var result =
                await grabber.GrabAsync(
                    new Uri("https://www.instagram.com/p/B4fk9vpnKdG/?utm_source=ig_web_button_share_sheet"));
            Assert.Equal(2, result.Resources.Count);
        }

        [Fact]
        public async void Test_YouTube()
        {
            var grabber = new YouTubeGrabber();
//            var result = await grabber.GrabAsync(new Uri("https://www.youtube.com/watch?v=hTbhdMAjh9Y"));
            var result = await grabber.GrabAsync(new Uri("https://www.youtube.com/watch?v=-BjZmE2gtdo"));
        }
    }
}