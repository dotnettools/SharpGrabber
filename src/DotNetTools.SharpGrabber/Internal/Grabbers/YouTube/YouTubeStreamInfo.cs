﻿namespace DotNetTools.SharpGrabber.Internal.Grabbers.YouTube
{
    /// <summary>
    /// Describes a YouTube stream.
    /// </summary>
    public class YouTubeStreamInfo
    {
        public int? iTag { get; set; }

        public string Type { get; set; }

        public string Mime { get; set; }

        /// <summary>
        /// Optional suggested extension of the file. If this value is NULL, the extension can be implied from <see cref="Mime"/>.
        /// </summary>
        public string Extension { get; set; }

        public string Quality { get; set; }

        public string Url { get; set; }

        public string Signature { get; set; }

        public string Decipher { get; set; }
    }
}