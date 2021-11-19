using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Internal
{
    internal static class EqualityUtils
    {
        public static bool Equals<T>(object object1, object object2, params Func<T, object>[] getValues)
            where T : class
        {
            // test references
            if (ReferenceEquals(object1, object2))
                return true;

            // test null values
            if (object1 == null || object2 == null)
                return false;

            // test different types
            var o1 = object1 as T;
            var o2 = object2 as T;
            if (o1 != null ^ o2 != null)
                return false;
            if (o1 == null && o2 == null)
                throw new ArgumentException($"Invalid type argument: {typeof(T)}", nameof(T));

            foreach (var getValue in getValues)
            {
                var val1 = getValue(o1);
                var val2 = getValue(o2);
                var areEqual = val1 == null ? val2 == null : val1.Equals(val2);
                if (!areEqual)
                    return false;
            }

            return true;
        }
    }
}
