using System.Collections.Generic;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Read-only collection of information about YouTube iTags.
    /// </summary>
    public static class YouTubeTags
    {
        private static readonly Dictionary<int, YouTubeTagInfo> _tags = new Dictionary<int, YouTubeTagInfo>();

        static YouTubeTags()
        {
            #region Tag Constants
            var tags =
                new[]
                {
                    new YouTubeTagInfo(5, "flv", true, true, "240p", null),
                    new YouTubeTagInfo(6, "flv", true, true, "270p", null),
                    new YouTubeTagInfo(17, "3gp", true, true, "144p", null),
                    new YouTubeTagInfo(18, "mp4", true, true, "360p", null),
                    new YouTubeTagInfo(22, "mp4", true, true, "720p", null),
                    new YouTubeTagInfo(34, "flv", true, true, "360p", null),
                    new YouTubeTagInfo(35, "flv", true, true, "480p", null),
                    new YouTubeTagInfo(36, "3gp", true, true, "180p", null),
                    new YouTubeTagInfo(37, "mp4", true, true, "1080p", null),
                    new YouTubeTagInfo(38, "mp4", true, true, "3072p", null),
                    new YouTubeTagInfo(43, "webm", true, true, "360p", null),
                    new YouTubeTagInfo(44, "webm", true, true, "480p", null),
                    new YouTubeTagInfo(45, "webm", true, true, "720p", null),
                    new YouTubeTagInfo(46, "webm", true, true, "1080p", null),
                    new YouTubeTagInfo(82, "mp4", true, true, "360p", null),
                    new YouTubeTagInfo(83, "mp4", true, true, "480p", null),
                    new YouTubeTagInfo(84, "mp4", true, true, "720p", null),
                    new YouTubeTagInfo(85, "mp4", true, true, "1080p", null),
                    new YouTubeTagInfo(92, "hls", true, true, "240p", null),
                    new YouTubeTagInfo(93, "hls", true, true, "360p", null),
                    new YouTubeTagInfo(94, "hls", true, true, "480p", null),
                    new YouTubeTagInfo(95, "hls", true, true, "720p", null),
                    new YouTubeTagInfo(96, "hls", true, true, "1080p", null),
                    new YouTubeTagInfo(100, "webm", true, true, "360p", null),
                    new YouTubeTagInfo(101, "webm", true, true, "480p", null),
                    new YouTubeTagInfo(102, "webm", true, true, "720p", null),
                    new YouTubeTagInfo(132, "hls", true, true, "240p", null),
                    new YouTubeTagInfo(133, "mp4", false, true, "240p", null),
                    new YouTubeTagInfo(134, "mp4", false, true, "360p", null),
                    new YouTubeTagInfo(135, "mp4", false, true, "480p", null),
                    new YouTubeTagInfo(136, "mp4", false, true, "720p", null),
                    new YouTubeTagInfo(137, "mp4", false, true, "1080p", null),
                    new YouTubeTagInfo(138, "mp4", false, true, "2160p60", null),
                    new YouTubeTagInfo(139, "m4a", true, false, null, 48),
                    new YouTubeTagInfo(140, "m4a", true, false, null, 128),
                    new YouTubeTagInfo(141, "m4a", true, false, null, 256),
                    new YouTubeTagInfo(151, "hls", true, true, "72p", null),
                    new YouTubeTagInfo(160, "mp4", false, true, "144p", null),
                    new YouTubeTagInfo(167, "webm", false, true, "360p", null),
                    new YouTubeTagInfo(168, "webm", false, true, "480p", null),
                    new YouTubeTagInfo(169, "webm", false, true, "1080p", null),
                    new YouTubeTagInfo(171, "webm", true, false, null, 128),
                    new YouTubeTagInfo(218, "webm", false, true, "480p", null),
                    new YouTubeTagInfo(219, "webm", false, true, "144p", null),
                    new YouTubeTagInfo(242, "webm", false, true, "240p", null),
                    new YouTubeTagInfo(243, "webm", false, true, "360p", null),
                    new YouTubeTagInfo(244, "webm", false, true, "480p", null),
                    new YouTubeTagInfo(245, "webm", false, true, "480p", null),
                    new YouTubeTagInfo(246, "webm", false, true, "480p", null),
                    new YouTubeTagInfo(247, "webm", false, true, "720p", null),
                    new YouTubeTagInfo(248, "webm", false, true, "1080p", null),
                    new YouTubeTagInfo(249, "webm", true, false, null, 50),
                    new YouTubeTagInfo(250, "webm", true, false, null, 70),
                    new YouTubeTagInfo(251, "webm", true, false, null, 160),
                    new YouTubeTagInfo(264, "mp4", false, true, "1440p", null),
                    new YouTubeTagInfo(266, "mp4", false, true, "2160p60", null),
                    new YouTubeTagInfo(271, "webm", false, true, "1440p", null),
                    new YouTubeTagInfo(272, "webm", false, true, "4320p", null),
                    new YouTubeTagInfo(278, "webm", false, true, "144p", null),
                    new YouTubeTagInfo(298, "mp4", false, true, "720p60", null),
                    new YouTubeTagInfo(299, "mp4", false, true, "1080p60", null),
                    new YouTubeTagInfo(302, "webm", false, true, "720p60", null),
                    new YouTubeTagInfo(303, "webm", false, true, "1080p60", null),
                    new YouTubeTagInfo(308, "webm", false, true, "1440p60", null),
                    new YouTubeTagInfo(313, "webm", false, true, "2160p", null),
                    new YouTubeTagInfo(315, "webm", false, true, "2160p60", null),
                    new YouTubeTagInfo(330, "webm", false, true, "144p60", null),
                    new YouTubeTagInfo(331, "webm", false, true, "240p60", null),
                    new YouTubeTagInfo(332, "webm", false, true, "360p60", null),
                    new YouTubeTagInfo(333, "webm", false, true, "480p60", null),
                    new YouTubeTagInfo(334, "webm", false, true, "720p60", null),
                    new YouTubeTagInfo(335, "webm", false, true, "1080p60", null),
                    new YouTubeTagInfo(336, "webm", false, true, "1440p60", null),
                    new YouTubeTagInfo(337, "webm", false, true, "2160p60", null),
                    new YouTubeTagInfo(394, "mp4", false, true, "144p", null),
                    new YouTubeTagInfo(395, "mp4", false, true, "240p", null),
                    new YouTubeTagInfo(396, "mp4", false, true, "360p", null),
                    new YouTubeTagInfo(397, "mp4", false, true, "480p", null),
                    new YouTubeTagInfo(398, "mp4", false, true, "720p", null),
                    new YouTubeTagInfo(399, "mp4", false, true, "1080p", null),
                    new YouTubeTagInfo(400, "mp4", false, true, "1440p", null),
                    new YouTubeTagInfo(401, "mp4", false, true, "2160p", null),
                    new YouTubeTagInfo(402, "mp4", false, true, "2880p", null),
                };
            #endregion

            foreach (var tagInfo in tags)
                _tags.Add(tagInfo.iTag, tagInfo);
        }

        /// <summary>
        /// Fetches <see cref="YouTubeTagInfo"/> for the specified iTag. Returns NULL if not found.
        /// </summary>
        public static YouTubeTagInfo? For(int iTag)
        {
            lock (_tags)
                return _tags.ContainsKey(iTag)
                    ? (YouTubeTagInfo?)_tags[iTag]
                    : null;
        }
    }

    /// <summary>
    /// Info about a YouTube iTag number
    /// </summary>
    public struct YouTubeTagInfo
    {
        #region Properties
        /// <summary>
        /// iTag number
        /// </summary>
        public int iTag { get; }

        /// <summary>
        /// Container format e.g. mp4, 3gp, hls, webm etc.
        /// </summary>
        public string Container { get; }

        /// <summary>
        /// Contains audio?
        /// </summary>
        public bool HasAudio { get; }

        /// <summary>
        /// Contains video?
        /// </summary>
        public bool HasVideo { get; }

        /// <summary>
        /// Video resolution e.g. 480p, 1080p, 2160p60
        /// Only applicable to video containers - NULL otherwise
        /// </summary>
        public string VideoResolution { get; }

        /// <summary>
        /// Bit rate e.g. 48 represents 48k - strictly audio only, not applicable to mux containers
        /// </summary>
        public int? BitRate { get; }
        #endregion

        #region Implied Properties
        /// <summary>
        /// Represents <see cref="BitRate"/> in string format.
        /// </summary>
        public string BitRateString => BitRate == null ? null : $"{BitRate}k";
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a read-only instance of <see cref="YouTubeTagInfo"/> with the specified properties.
        /// </summary>
        public YouTubeTagInfo(int iTag, string container, bool hasAudio, bool hasVideo, string videoResolution, int? bitRate)
        {
            this.iTag = iTag;
            Container = container;
            HasAudio = hasAudio;
            HasVideo = hasVideo;
            VideoResolution = videoResolution;
            BitRate = bitRate;
        }
        #endregion
    }
}