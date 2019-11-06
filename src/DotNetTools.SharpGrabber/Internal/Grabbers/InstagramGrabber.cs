using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Internal.Grabbers
{
    /// <summary>
    /// Represents an Instagram <see cref="IGrabber"/>.
    /// </summary>
    public class InstagramGrabber : BaseGrabber
    {
        #region Methods
        public async override Task<IEnumerable<IGrabbed>> Grab(Uri uri, GrabOptions options)
        {
            // init
            Status.Update(null, null);

            // download target page
            Status.Update(null, "Downloading page...", WorkStatusType.DownloadingPage);
            var client = HttpHelper.CreateClient(uri);
            var response = await client.GetAsync(uri);

            throw new NotImplementedException();
        }
        #endregion
    }
}
