using DotNetTools.SharpGrabber.Hls;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Reference to an M3U8 playlist.
    /// </summary>
    public class GrabbedStreamReference : IGrabbed
    {
        public GrabbedStreamReference(Uri resourceUri, Uri originalUri)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
        }

        public Uri OriginalUri { get; }

        public Uri ResourceUri { get; }

        public HlsPlaylistType PlaylistType { get; set; }

        public string Resolution { get; set; }
    }
}
