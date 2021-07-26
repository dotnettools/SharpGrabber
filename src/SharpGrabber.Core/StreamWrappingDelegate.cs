using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Accepts an stream and wraps it with other streams if and when necessary.
    /// </summary>
    public delegate Task<Stream> StreamWrappingDelegate(Stream stream);
}
