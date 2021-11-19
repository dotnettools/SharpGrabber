using System.Collections.Generic;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Contains helper methods for dictionaries.
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// Gets and returns value associated with the specified key in the dictionary. Returns <paramref name="defaultValue"/> if no entry with the specified
        /// key exists.
        /// </summary>
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
}
