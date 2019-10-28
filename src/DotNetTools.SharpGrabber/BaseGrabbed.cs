using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    internal abstract class BaseGrabbed : IGrabbed
    {
        #region Properties
        public Uri Uri { get; }
        #endregion

        #region Constructor
        public BaseGrabbed(Uri uri)
        {
            Uri = uri;
        }
        #endregion
    }
}
