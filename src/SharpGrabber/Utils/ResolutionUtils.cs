using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetTools.SharpGrabber.Utils
{
    public static class ResolutionUtils
    {
        private static readonly VideoResolutionDescriptor[] Resolutions = new VideoResolutionDescriptor[]
        {
            new("360p", null, "SD", 360),
            new("480p", null, "SD", 480),
            new("576p", null, "SD", 576),
            new("720p", null, "HD", 720),
            new("1080p", null, "Full HD", 1080),
            new("1440p", null, "Quad HD", 1140),
            new("2K", null, null, 1080),
            new("4K", "2160p", "Ultra HD", 2160),
            new("8K", "4320p", "Full Ultra HD", 4320),
        };

        private static readonly Dictionary<string, VideoResolutionDescriptor> ResolutionsByName;

        static ResolutionUtils()
        {
            ResolutionsByName =
                new Dictionary<string, VideoResolutionDescriptor>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var resolution in Resolutions)
            {
                ResolutionsByName.Add(resolution.Name, resolution);
                if (!string.IsNullOrEmpty(resolution.AlternativeName))
                    ResolutionsByName.Add(resolution.AlternativeName, resolution);
            }
        }

        /// <summary>
        /// Enumerates the descriptors of all standard resolutions.
        /// </summary>
        public static IEnumerable<VideoResolutionDescriptor> All()
        {
            return Resolutions.AsEnumerable();
        }

        /// <summary>
        /// Tries to find a resolution descriptor by its name e.g. 480p
        /// </summary>
        public static VideoResolutionDescriptor Find(string resolution)
        {
            return ResolutionsByName.GetOrDefault(resolution);
        }

        /// <summary>
        /// Finds the resolution descriptor(s) with height of <paramref name="resolutionHeight"/>. 
        /// </summary>
        public static VideoResolutionDescriptor Find(int resolutionHeight)
        {
            return Resolutions
                .FirstOrDefault(r => r.PixelHeight == resolutionHeight);
        }
    }
}