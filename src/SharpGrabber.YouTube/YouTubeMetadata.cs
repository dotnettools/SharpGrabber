using System.Collections.Generic;
using System.Linq;

namespace DotNetTools.SharpGrabber.YouTube.YouTube
{
    /// <summary>
    /// Represents metadata of a YouTube video.
    /// </summary>
    public class YouTubeMetadata
    {
        #region Properties
        /// <summary>
        /// status - e.g. ok
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// fmt_list - e.g. 18/640x360
        /// </summary>
        public string FormatList { get; set; }

        /// <summary>
        /// url_encoded_fmt_stream_map: Multiplexed streams
        /// </summary>
        public List<YouTubeMuxedStream> MuxedStreams { get; set; }

        /// <summary>
        /// adaptive_fmts: Video-only and audio-only streams
        /// </summary>
        public List<YouTubeAdaptiveStream> AdaptiveStreams { get; set; }

        /// <summary>
        /// player_response: YouTube player metadata
        /// </summary>
        public YouTubePlayerResponse PlayerResponse { get; set; } = new YouTubePlayerResponse();
        #endregion

        #region Implied Properties
        /// <summary>
        /// Concatenates <see cref="MuxedStreams"/> with <see cref="AdaptiveStreams"/> and returns the result as
        /// an enumerable.
        /// </summary>
        public IEnumerable<YouTubeStreamInfo> AllStreams => MuxedStreams.Concat<YouTubeStreamInfo>(AdaptiveStreams);
        #endregion
    }
}