using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    /// <summary>
    /// Represents a factory of <typeparamref name="T"/>.
    /// </summary>
    public interface IFactory<T>
    {
        #region Methods
        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>.
        /// </summary>
        T Create();
        #endregion
    }
}
