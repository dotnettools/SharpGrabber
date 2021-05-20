using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    /// <summary>
    /// A simple implementation for <see cref="IFactory{T}"/> that makes instances of
    /// <typeparamref name="TImplementationType"/>.
    /// </summary>
    public class TypeFactory<TImplementationType, TFactoryType> : TypeFactory<TFactoryType>
        where TImplementationType : TFactoryType
    {
        public TypeFactory() : base(typeof(TImplementationType))
        {
        }
    }

    /// <summary>
    /// A simple implementation for <see cref="IFactory{T}"/> that makes instances of <see cref="ImplementationType"/>.
    /// </summary>
    public class TypeFactory<TFactoryType> : IFactory<TFactoryType>
    {
        public TypeFactory(Type implementationType)
        {
            ImplementationType = implementationType;
        }

        public Type ImplementationType { get; }

        /// <inheritdoc />
        public TFactoryType Create()
            => (TFactoryType)Activator.CreateInstance(ImplementationType);
    }
}
