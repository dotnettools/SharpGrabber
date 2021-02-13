using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Hls
{
    public class HlsKey
    {
        public HlsKey(HlsKeyMethod method, Uri uri, byte[] iv)
        {
            Method = method;
            Uri = uri;
            Iv = iv;
        }

        public HlsKeyMethod Method { get; }
        public Uri? Uri { get; }
        public byte[] Iv { get; }
    }
}
