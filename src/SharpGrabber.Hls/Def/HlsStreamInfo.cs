﻿using System;

namespace DotNetTools.SharpGrabber.Hls
{
    public class HlsStreamInfo
    {
        public HlsStreamInfo(Uri uri, int programId, int bandwidth, RectSize resolution, string name)
        {
            ProgramId = programId;
            Bandwidth = bandwidth;
            Resolution = resolution;
            Name = name;
            Uri = uri;
        }

        public int ProgramId { get; }
        public int Bandwidth { get; }
        public RectSize Resolution { get; }
        public string Name { get; }
        public Uri Uri { get; }
    }
}
