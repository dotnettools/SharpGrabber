using System;

namespace DotNetTools.SharpGrabber.Internal
{
    internal static class StringHelper
    {
        /// <summary>
        /// Same as <see cref="Uri.UnescapeDataString"/> but also converts plus to space.
        /// </summary>
        public static string DecodeUriString(string s)
        {
            return Uri.UnescapeDataString(s)
                .Replace('+', ' ');
        }
    }
}