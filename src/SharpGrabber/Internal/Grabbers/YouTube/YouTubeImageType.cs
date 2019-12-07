using System;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Represents YouTube image type - such as hq, mq, sd, maxres etc.
    /// </summary>
    public enum YouTubeImageType
    {
        /// <summary>
        /// default image
        /// </summary>
        Default,

        /// <summary>
        /// hqdefault image
        /// </summary>
        HighQuality,

        /// <summary>
        /// mqdefault image
        /// </summary>
        MediumQuality,

        /// <summary>
        /// sddefault image
        /// </summary>
        StandardQuality,

        /// <summary>
        /// maxresdefault image
        /// </summary>
        MaximumResolution,
    }

    /// <summary>
    /// Contains helper methods for <see cref="YouTubeImageType"/>.
    /// </summary>
    public static class YouTubeImageTypeHelper
    {
        /// <summary>
        /// Returns representation of this <see cref="YouTubeImageType"/> in YouTube links; such as 'hqdefault'.
        /// </summary>
        public static string ToYouTubeString(this YouTubeImageType type)
        {
            switch (type)
            {
                case YouTubeImageType.Default:
                    return "default";

                case YouTubeImageType.MaximumResolution:
                    return "maxresdefault";

                case YouTubeImageType.HighQuality:
                    return "hqdefault";

                case YouTubeImageType.MediumQuality:
                    return "mqdefault";

                case YouTubeImageType.StandardQuality:
                    return "sddefault";

                default:
                    throw new NotSupportedException($"YouTube image type '{type}' is undefined for the helper.");
            }
        }
    }
}