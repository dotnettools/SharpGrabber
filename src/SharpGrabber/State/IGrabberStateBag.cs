using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Stores client's state.
    /// </summary>
    public interface IGrabberStateBag
    {
        /// <summary>
        /// Gets the state associated with <paramref name="key"/>.
        /// </summary>
        object Get(object key, object @default = null);

        /// <summary>
        /// Sets the state to <paramref name="value"/> specified by <paramref name="key"/>.
        /// </summary>
        void Set(object key, object value);

        /// <summary>
        /// Deletes a state entry by <paramref name="key"/>.
        /// </summary>
        void Delete(object key);
    }

    /// <summary>
    /// Defines extension methods for <see cref="IGrabberStateBag"/>.
    /// </summary>
    public static class GrabberStateBagExtensions
    {
        /// <summary>
        /// Gets the state associated with <paramref name="key"/> of type <typeparamref name="T"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this IGrabberStateBag bag, object key, T @default = default)
        {
            return (T)bag.Get(key, @default);
        }
    }
}
