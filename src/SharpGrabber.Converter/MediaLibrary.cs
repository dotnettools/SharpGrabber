using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Converter
{
    public static class MediaLibrary
    {
        public static void Load(string dir)
        {
            ffmpeg.RootPath = dir;
        }

        public static string FFMpegVersion() => ffmpeg.av_version_info();
    }
}
