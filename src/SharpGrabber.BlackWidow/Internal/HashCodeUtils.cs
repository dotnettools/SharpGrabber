using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Internal
{
    internal static class HashCodeUtils
    {
        public static int Compute(params object[] values)
        {
            return ComputeCustom(values);
        }

        private static int ComputeCustom(object[] values)
        {
            unchecked
            {
                var hash = 17;
                foreach (var value in values)
                {
                    var ohash = value?.GetHashCode() ?? 0;
                    hash = hash * 23 + ohash;
                }
                return hash;
            }
        }
    }
}
