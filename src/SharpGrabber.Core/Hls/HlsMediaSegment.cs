using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Hls
{
    public class HlsMediaSegment
    {
        public HlsMediaSegment(Uri uri, TimeSpan duration, string title = null)
        {
            Uri = uri;
            Duration = duration;
            Title = title;
        }

        /// <summary>
        /// Optional title of the segment
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Duration of the segment
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// URI of the segment file - may be either relative or absolute
        /// </summary>
        public Uri Uri { get; }
    }
}
