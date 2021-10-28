using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Utils;

namespace DotNetTools.SharpGrabber
{
    public static class GrabbedMediaExtensions
    {
        private static readonly Regex BitRateParseRegex =
            new Regex(@"^\s*([\d\,]+)\s*([kmg]?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// If <see cref="GrabbedMedia.Resolution"/> is available and it's a standard resolution,
        /// gets the corresponding <see cref="VideoResolutionDescriptor"/>. 
        /// </summary>
        public static VideoResolutionDescriptor GetResolutionDescriptor(this GrabbedMedia media)
        {
            if (!string.IsNullOrEmpty(media.Resolution))
                return ResolutionUtils.Find(media.Resolution);

            if (media.PixelHeight.HasValue)
                return ResolutionUtils.Find(media.PixelHeight.Value);

            return null;
        }

        /// <summary>
        /// If <see cref="GrabbedMedia.BitRateString"/> is available, gets the bitrate as number.
        /// </summary>
        public static int? GetBitRate(this GrabbedMedia media)
        {
            if (string.IsNullOrEmpty(media.BitRateString))
                return null;

            var match = BitRateParseRegex.Match(media.BitRateString);
            if (!match.Success)
                return null;

            var result = int.Parse(match.Groups[1].Value, NumberStyles.Any);
            if (!string.IsNullOrEmpty(match.Groups[2].Value))
                result *= ParseMetric(match.Groups[2].Value);

            return result;
        }

        /// <summary>
        /// Sorts the media resources by their quality in ascending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> OrderByResolution(this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.OrderBy(m => m.GetResolutionDescriptor());
        }

        /// <summary>
        /// Sorts the media resources by their quality in ascending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> ThenByResolution(this IOrderedEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.ThenBy(m => m.GetResolutionDescriptor());
        }

        /// <summary>
        /// Sorts the media resources by their quality in descending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> OrderByResolutionDescending(
            this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.OrderByDescending(m => m.GetResolutionDescriptor());
        }

        /// <summary>
        /// Sorts the media resources by their quality in descending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> ThenByResolutionDescending(
            this IOrderedEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.ThenByDescending(m => m.GetResolutionDescriptor());
        }

        /// <summary>
        /// Sorts the media resources by their bitrate in ascending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> OrderByBitRate(this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.OrderBy(m => m.GetBitRate());
        }

        /// <summary>
        /// Sorts the media resources by their bitrate in ascending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> ThenByBitRate(this IOrderedEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.ThenBy(m => m.GetBitRate());
        }

        /// <summary>
        /// Sorts the media resources by their bitrate in descending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> OrderByBitRateDescending(this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.OrderByDescending(m => m.GetBitRate());
        }

        /// <summary>
        /// Sorts the media resources by their bitrate in descending order.
        /// </summary>
        public static IOrderedEnumerable<GrabbedMedia> ThenByBitRateDescending(
            this IOrderedEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.ThenByDescending(m => m.GetBitRate());
        }

        /// <summary>
        /// Filters only media resources with video channel. 
        /// </summary>
        public static IEnumerable<GrabbedMedia> WhereHasVideo(this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.Where(m => m.Channels.HasVideo());
        }

        /// <summary>
        /// Filters only media resources with audio channel. 
        /// </summary>
        public static IEnumerable<GrabbedMedia> WhereHasAudio(this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed.Where(m => m.Channels.HasAudio());
        }

        /// <summary>
        /// Gets the media containing video channel with the most quality.
        /// </summary>
        public static GrabbedMedia GetHighestQualityVideo(
            this IEnumerable<GrabbedMedia> grabbed)
        {
            GrabbedMedia result = null;
            var highestHeight = 0;
            foreach (var media in grabbed.WhereHasVideo())
            {
                var pixelHeight = media.PixelHeight ?? media.GetResolutionDescriptor()?.PixelHeight ?? 0;
                if (highestHeight > 0 && pixelHeight <= highestHeight) continue;
                highestHeight = pixelHeight;
                result = media;
            }

            return result;
        }

        /// <summary>
        /// Gets the media containing video channel with the most quality.
        /// </summary>
        public static GrabbedMedia GetHighestQualityAudio(
            this IEnumerable<GrabbedMedia> grabbed)
        {
            GrabbedMedia result = null;
            var highestBitRate = -1;
            foreach (var media in grabbed.WhereHasAudio())
            {
                var bitrate = media.GetBitRate();
                if (highestBitRate >= 0 && (!bitrate.HasValue || bitrate.Value <= highestBitRate))
                    continue;
                highestBitRate = bitrate ?? 0;
                result = media;
            }

            return result;
        }

        /// <summary>
        /// Gets the media containing both video and audio channels with the best quality, if any.
        /// </summary>
        public static GrabbedMedia GetHighestQualityMux(
            this IEnumerable<GrabbedMedia> grabbed)
        {
            return grabbed
                .Where(m => m.Channels == MediaChannels.Both)
                .OrderByResolutionDescending()
                .ThenByBitRateDescending()
                .FirstOrDefault();
        }

        private static int ParseMetric(string metric)
        {
            if (string.IsNullOrEmpty(metric))
                return 1;
            return char.ToLowerInvariant(metric[0]) switch
            {
                'k' => 1000,
                'm' => 1000000,
                'g' => 1000000000,
                _ => throw new FormatException($"Could not parse metric ({metric}).")
            };
        }
    }
}