using DotNetTools.SharpGrabber.Hls;
using System;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Reference to an M3U8 playlist.
    /// </summary>
    [GrabbedType("HlsStreamReference")]
    public class GrabbedHlsStreamReference : IGrabbed
    {
        public GrabbedHlsStreamReference()
        {
        }

        public GrabbedHlsStreamReference(Uri resourceUri)
        {
            ResourceUri = resourceUri;
        }

        [Obsolete("Use another constructor.")]
        public GrabbedHlsStreamReference(Uri resourceUri, Uri originalUri)
        {
            OriginalUri = originalUri;
            ResourceUri = resourceUri;
        }

        public Uri OriginalUri { get; }

        public Uri ResourceUri { get; set; }

        public HlsPlaylistType PlaylistType { get; set; }

        public string Resolution { get; set; }
    }
}
