using DotNetTools.SharpGrabber.Internal.Grabbers;
using System;
using Xunit;

namespace DotNetTools.SharpGrabber.Tests
{
    public class GrabTests
    {
        [Fact]
        public async void Test_Instagram_VideoClip()
        {
            var grabber = new InstagramGrabber();
            var result =
                await grabber.GrabAsync(
                    new Uri("https://www.instagram.com/p/CAs5qr7gYQJ/?utm_source=ig_web_copy_link"));
            Assert.Equal(2, result.Resources.Count);
        }

        [Fact]
        public async void Test_YouTube_Normal()
        {
            var grabber = new YouTubeGrabber();
            var result = await grabber.GrabAsync(new Uri("https://www.youtube.com/watch?v=ThUS9fj-mCA"));
            Assert.False(result.IsSecure);
            Assert.Equal(23, result.Resources.Count);
        }

        [Fact]
        public async void Test_YouTube_Decipher()
        {
            var grabber = new YouTubeGrabber();
            var result = await grabber.GrabAsync(new Uri("https://www.youtube.com/watch?v=ZR02k-h0lX0"));
            Assert.True(result.IsSecure);
            Assert.Equal(33, result.Resources.Count);
        }
    }
}