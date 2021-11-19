using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Internal
{
    internal static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length == 1)
                return str.ToLowerInvariant();

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
