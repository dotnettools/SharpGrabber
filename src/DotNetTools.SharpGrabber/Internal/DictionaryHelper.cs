using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    /// <summary>
    /// Contains helper methods for dictionaries.
    /// </summary>
    internal static class DictionaryHelper
    {
        /// <summary>
        /// Gets and returns value associated with the specified key in the dictionary. If no entry with the specified
        /// key exists, <paramref name="defaultValue"/> is returned.
        /// </summary>
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            if (!dictionary.ContainsKey(key))
                return defaultValue;
            return dictionary[key];
        }
    }
}
