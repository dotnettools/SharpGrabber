using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Configures a repository on a builder.
    /// </summary>
    public interface IBlackWidowRepositoryConfigurator
    {
        /// <summary>
        /// Gets the configured repository.
        /// </summary>
        IGrabberRepository Repository { get; }

        /// <summary>
        /// Uses a repository instance.
        /// </summary>
        IBlackWidowRepositoryConfigurator Use(IGrabberRepository repository);
    }
}
