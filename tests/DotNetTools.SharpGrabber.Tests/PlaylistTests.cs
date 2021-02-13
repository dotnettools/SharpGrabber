using DotNetTools.SharpGrabber.Hls;
using DotNetTools.SharpGrabber.Internal.Grabbers;
using DotNetTools.SharpGrabber.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.Tests
{
    public class PlaylistTests
    {
        [Theory]
        [InlineData(@"https://hls-hw.xnxx-cdn.com/videos/hls/33/74/bb/3374bb0ec93b66310f74e4e312b6d8f5/hls.m3u8?e=1613230337&l=0&h=d55ff0e1628610d73812e1b76139fdda")]
        //[InlineData(@"https://hls-hw.xnxx-cdn.com/videos/hls/33/74/bb/3374bb0ec93b66310f74e4e312b6d8f5/hls-720p-aa00e.m3u8?e=1613230337&l=0&h=d55ff0e1628610d73812e1b76139fdda")]
        public async Task TestLoad(string url)
        {
            var uri = new Uri(url);
            var g = new HlsGrabber();
            try
            {
                var r = await g.GrabAsync(uri);
                var x = r.Resources.OfType<GrabbedStreamMetadata>().Where(m => m.Resolution.Height == 720).Single();
                var stream = await x.Stream.ResolveAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
