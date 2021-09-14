using System;
using System.Collections.Generic;

namespace DotNetTools.SharpGrabber.YouTube
{
    /// <summary>
    /// Provides helper methods for usage in YouTube module.
    /// </summary>
    internal static class YouTubeUtils
    {
        /// <summary>
        /// Extracts a URL encoded string into a list of key value pairs.
        /// </summary>
        public static List<KeyValuePair<string, string>> ExtractUrlEncodedParamList(string encodedString)
        {
            var result = new List<KeyValuePair<string, string>>();
            var encodedPairs = encodedString.Split('&');

            foreach (var encodedPair in encodedPairs)
            {
                var splittedPair = encodedPair.Split(new[] {'='}, 2);
                if (splittedPair.Length != 2)
                    // ignore possibly invalid pair
                    continue;
                splittedPair[0] = Uri.UnescapeDataString(splittedPair[0]);
                splittedPair[1] = Uri.UnescapeDataString(splittedPair[1]);
                result.Add(new KeyValuePair<string, string>(splittedPair[0], splittedPair[1]));
            }

            return result;
        }

        /// <summary>
        /// Extracts a URL encoded string into a map.
        /// </summary>
        public static Dictionary<string, string> ExtractUrlEncodedParamMap(string encodedString, bool ignoreDuplicates = true)
        {
            var map = new Dictionary<string, string>();
            var list = ExtractUrlEncodedParamList(encodedString);
            foreach (var par in list)
                if (map.ContainsKey(par.Key))
                {
                    if (!ignoreDuplicates)
                        throw new InvalidOperationException($"Duplicate key [{par.Key}] found in URL encoded string.");
                }
                else
                    map.Add(par.Key, par.Value);

            return map;
        }
    }
}