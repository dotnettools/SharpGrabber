using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Configures a repository on a builder.
    /// </summary>
    public interface IBlackWidowRepositoryBuilder
    {
        /// <summary>
        /// Uses a repository instance.
        /// </summary>
        IBlackWidowRepositoryBuilder Use(IGrabberRepository repository);
    }
}
