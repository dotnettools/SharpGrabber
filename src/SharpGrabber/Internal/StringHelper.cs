using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    internal static class StringHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }
    }
}
