using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    internal abstract class BaseGrabber : IGrabber
    {
        protected string[] SupportedSchemes { get; }

        public string[] GetSupportedSchemes() => SupportedSchemes;

        public abstract Task<IEnumerable<IGrabbed>> Grab(Uri uri);
    }
}
