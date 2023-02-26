using DotNetTools.SharpGrabber;
using SharpGrabber.Odysee;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpGrabber.Tests.Odysee
{
    public class OdyseeTests
    {
        [Fact]
        public async Task Test_GrabFromUrl_ExpectSuccess()
        {
            var grabber = new OdyseeGrabber(GrabberServices.Default);

            var uri = new Uri(@"https://odysee.com/@shroudedhand:a/one-of-britain's-most-disturbing-legends:d");

            var result = await grabber.GrabAsync(uri).ConfigureAwait(false);
            Assert.NotEmpty(result.Title);
            Assert.NotEmpty(result.Description);
            Assert.True(result.Resources.Count >= 3);
        }
    }
}
