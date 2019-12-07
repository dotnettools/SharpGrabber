using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFmpeg.AutoGen;

namespace DotNetTools.SharpGrabber.Converter
{
    public static class MediaHelper
    {
        public static AVHWDeviceType[] GetHardwareDeviceTypes()
        {
            var set = new HashSet<AVHWDeviceType>();
            var type = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;
            while ((type = ffmpeg.av_hwdevice_iterate_types(type)) != AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
                set.Add(type);
            return set.ToArray();
        }
    }
}
