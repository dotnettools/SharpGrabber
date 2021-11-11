using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    internal class ConcurrentHashSet<T> : ISet<T>
    {
        private readonly ConcurrentDictionary<T, bool> _dic;

        public ConcurrentHashSet(IEnumerable<T> items)
        {
            var pairs = items
                .Select(item => new KeyValuePair<T, bool>(item, false));
            _dic = new ConcurrentDictionary<T, bool>(pairs);
        }

        public ConcurrentHashSet() : this(Array.Empty<T>())
        {
        }

        public int Count => _dic.Count;

        public bool IsReadOnly => (_dic as IDictionary<T, bool>).IsReadOnly;

        public bool Add(T item)
            => _dic.TryAdd(item, false);

        public void Clear()
            => _dic.Clear();

        public bool Contains(T item)
            => _dic.ContainsKey(item);

        public void CopyTo(T[] array, int arrayIndex)
            => _dic.Keys.CopyTo(array, arrayIndex);

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                _dic.TryRemove(item, out _);
        }

        public IEnumerator<T> GetEnumerator()
            => _dic.Keys.GetEnumerator();

        public void IntersectWith(IEnumerable<T> other)
        {
            var otherSet = other as ISet<T> ?? new HashSet<T>(other);
            foreach (var item in _dic.Keys.Where(k => !otherSet.Contains(k)))
                _dic.TryRemove(item, out _);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            var otherSet = new HashSet<T>(other);
            return otherSet.Count > Count && !_dic.Keys.Any(k => !otherSet.Contains(k));
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return Count > other.Count() && !other.Any(k => !_dic.ContainsKey(k));
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var otherSet = new HashSet<T>(other);
            return otherSet.Count >= Count && !_dic.Keys.Any(k => !otherSet.Contains(k));
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return !other.Any(k => !_dic.ContainsKey(k));
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return other.Any(item => _dic.ContainsKey(item));
        }

        public bool Remove(T item)
            => _dic.TryRemove(item, out _);

        public bool SetEquals(IEnumerable<T> other)
            => other.Count() == Count && other.All(item => _dic.ContainsKey(item));

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                if (!_dic.TryRemove(item, out _))
                    _dic.TryAdd(item, false);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
                _dic.TryAdd(item, false);
        }

        void ICollection<T>.Add(T item)
            => _dic.TryAdd(item, false);

        IEnumerator IEnumerable.GetEnumerator()
            => _dic.Keys.GetEnumerator();
    }
}
