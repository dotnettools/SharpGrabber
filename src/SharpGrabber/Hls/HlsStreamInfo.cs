using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Hls
{
    public class HlsStreamInfo
    {
        public HlsStreamInfo(Uri uri, int programId, int bandwidth, Size resolution, string name)
        {
            ProgramId = programId;
            Bandwidth = bandwidth;
            Resolution = resolution;
            Name = name;
            Uri = uri;
        }

        public int ProgramId { get; }
        public int Bandwidth { get; }
        public Size Resolution { get; }
        public string Name { get; }
        public Uri Uri { get; }
    }
}
