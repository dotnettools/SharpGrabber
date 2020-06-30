using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    /// <summary>
    /// A simple implementation for <see cref="IFactory{T}"/> that makes instances of
    /// <typeparamref name="TImplementationType"/>.
    /// </summary>
    public class TypeFactory<TImplementationType, TFactoryType> : IFactory<TFactoryType>
        where TImplementationType : TFactoryType
    {
        #region Methods
        /// <inheritdoc />
        public TFactoryType Create()
            => Activator.CreateInstance<TImplementationType>();
        #endregion
    }
}
