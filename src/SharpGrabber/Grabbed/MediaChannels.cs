using System;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Represents stream channels of a media file - audio, video, or both.
    /// </summary>
    [Flags]
    public enum MediaChannels
    {
        /// <summary>
        /// No channel (not for practical purposes)
        /// </summary>
        /// <remarks>
        /// No <see cref="IGrabber"/> is supposed to return a media with no channels.
        /// <see cref="None"/> is there to make flags valid and it might be useful for the user.
        /// </remarks>
        None = 0,

        /// <summary>
        /// Video channel
        /// </summary>
        Video = 1,

        /// <summary>
        /// Audio channel
        /// </summary>
        Audio = 2,

        /// <summary>
        /// Audio and video channels
        /// </summary>
        Both = 2 | 1,
    }

    /// <summary>
    /// Contains helper methods for <see cref="MediaChannels"/>.
    /// </summary>
    public static class MediaChannelsHelper
    {
        /// <summary>
        /// Whether or not an audio channel is available.
        /// </summary>
        public static bool HasAudio(this MediaChannels channels) => channels.HasFlag(MediaChannels.Audio);

        /// <summary>
        /// Whether or not a video channel is available.
        /// </summary>
        public static bool HasVideo(this MediaChannels channels) => channels.HasFlag(MediaChannels.Video);
    }
}