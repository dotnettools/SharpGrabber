using System;
using Xunit;

namespace DotNetTools.SharpGrabber.Tests
{
    public class MultiGrabberTests
    {
        [Fact]
        public async void Test_Vimeo()
        {
            var grabber = MultiGrabber.CreateDefault();
            var result = await grabber.Grab(new Uri("https://vimeo.com/88991219"));

            Assert.Equal("SHORT", result.Title);
            Assert.Equal(6, result.Resources.Count);
        }
    }
}
