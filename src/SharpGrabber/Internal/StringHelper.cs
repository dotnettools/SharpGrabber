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

        public static int ForceParseInt(string s)
        {
            var num = 0;
            foreach (var ch in s)
            {
                if (!char.IsDigit(ch))
                    continue;
                var digit = (int)char.GetNumericValue(ch);
                num = num * 10 + digit;
            }
            return num;
        }
    }
}