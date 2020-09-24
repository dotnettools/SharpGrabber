using DotNetTools.SharpGrabber.Internal.Grabbers;
using DotNetTools.SharpGrabber.Media;
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
            // var result = await grabber.GrabAsync(new Uri("https://www.youtube.com/watch?v=-BjZmE2gtdo"));
            var result = await grabber.GrabAsync(new Uri("https://www.youtube.com/watch?v=Fpgd3ac3_nM"));
        }

        [Fact]
        public async void Test_Vimeo()
        {
            var grabber = new VimeoGrabber();
            var result = await grabber.GrabAsync(new Uri("https://vimeo.com/88991219"));

            Assert.Equal("SHORT", result.Title);
            Assert.Equal(6, result.Resources.Count);

            Assert.IsType<GrabbedImage>(result.Resources[0]);
            Assert.Equal("https://i.vimeocdn.com/video/467580616_1280.jpg", result.Resources[0].ResourceUri.ToString());

            Assert.IsType<GrabbedImage>(result.Resources[1]);
            Assert.Equal("https://i.vimeocdn.com/video/467580616_960.jpg", result.Resources[1].ResourceUri.ToString());

            Assert.IsType<GrabbedImage>(result.Resources[2]);
            Assert.Equal("https://i.vimeocdn.com/video/467580616_640.jpg", result.Resources[2].ResourceUri.ToString());

            Assert.IsType<GrabbedImage>(result.Resources[3]);
            Assert.Equal("https://i.vimeocdn.com/video/467580616", result.Resources[3].ResourceUri.ToString());

            Assert.IsType<GrabbedMedia>(result.Resources[4]);
            var grabbedMedia = (GrabbedMedia)result.Resources[4];
            Assert.NotNull(grabbedMedia.ResourceUri);
            Assert.Equal("video/mp4", grabbedMedia.Format.Mime);
            Assert.Equal("mp4", grabbedMedia.Format.Extension);
            Assert.Equal("720p", grabbedMedia.Resolution);

            Assert.IsType<GrabbedMedia>(result.Resources[5]);
            grabbedMedia = (GrabbedMedia)result.Resources[5];
            Assert.NotNull(grabbedMedia.ResourceUri);
            Assert.Equal("video/mp4", grabbedMedia.Format.Mime);
            Assert.Equal("mp4", grabbedMedia.Format.Extension);
            Assert.Equal("360p", grabbedMedia.Resolution);
        }
    }
}
