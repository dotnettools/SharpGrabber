using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Represents a value that is lazy loaded.
    /// </summary>
    /// <remarks>Implementers are expected to be thread-safe.</remarks>
    public interface IResolvable<T>
    {
        /// <summary>
        /// Indicates whether or not the value is resolved.
        /// </summary>
        bool IsResolved { get; }

        /// <summary>
        /// Asynchronously resolves the value if it is not already resolved.
        /// </summary>
        Task<T> Resolve();
    }

    /// <remarks>
    /// Methods of this class are thread-safe.
    /// </remarks>
    public abstract class BaseResolvable<T> : IResolvable<T>
    {
        #region Fields
        private T _value;
        private bool _resolved = false;
        private Task<T> _resolving;
        private readonly object _internalLock = new object();
        #endregion

        #region Constructors
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public BaseResolvable() { }

        public BaseResolvable(T resolvedValue)
        {
            _value = resolvedValue;
            _resolved = true;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region Internal Methods
        /// <summary>
        /// This method gets invoked when the value is being resolved for the first time.
        /// </summary>
        protected abstract Task<T> InternalResolve();
        #endregion

        #region Methods
        /// <inheritdoc />
        public bool IsResolved
        {
            get
            {
                lock (_internalLock)
                    return _resolved;
            }
        }

        /// <inheritdoc />
        public Task<T> Resolve()
        {
            var task = new TaskCompletionSource<T>();

            try
            {
                lock (_internalLock)
                {
                    // test if the value is already resolved
                    if (_resolved)
                    {
                        task.SetResult(_value);
                        return task.Task;
                    }

                    // test if the value is being resolved by another simultaneous invocation
                    if (_resolving != null)
                        return _resolving;

                    // return resolving method
                    _resolving = InternalResolve();
                }

                // update cached value when internal resolve process is done
                _resolving.ContinueWith(_resolvingTask =>
                {
                    lock (_internalLock)
                    {
                        _resolved = true;
                        _value = _resolvingTask.Result;
                    }
                });

                return _resolving;
            }
            finally
            {
                lock (_internalLock)
                    _resolving = null;
            }
        }
        #endregion
    }
}
