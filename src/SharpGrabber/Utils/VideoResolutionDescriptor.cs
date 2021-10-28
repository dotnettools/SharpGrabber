using System;

namespace DotNetTools.SharpGrabber.Utils
{
    /// <summary>
    /// Describes a standard video resolution.
    /// </summary>
    /// <remarks>
    /// A complete list of display resolutions can be found at https://en.wikipedia.org/wiki/Display_resolution. 
    /// </remarks>
    public class VideoResolutionDescriptor : IComparable<VideoResolutionDescriptor>
    {
        public VideoResolutionDescriptor(string name, string alternativeName, string description,
            int pixelHeight)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            AlternativeName = alternativeName;
            Description = description;
            PixelHeight = pixelHeight;
        }

        /// <summary>
        /// Gets the common name for this resolution e.g. '4K'
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the alternative name for this resolution e.g. '4320p' if available, or NULL.
        /// </summary>
        public string AlternativeName { get; }

        /// <summary>
        /// Gets the optional description of this resolution e.g. 'Ultra HD' or NULL if unavailable.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the pixel width e.g. 1080
        /// </summary>
        public int PixelHeight { get; }

        public int CompareTo(VideoResolutionDescriptor other)
        {
            if (other == null)
                return 1;
            return PixelHeight.CompareTo(other.PixelHeight);
        }

        public override string ToString()
            => $"{Name} {Description}".Trim();
    }
}