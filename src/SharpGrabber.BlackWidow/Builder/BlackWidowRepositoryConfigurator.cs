using DotNetTools.SharpGrabber.BlackWidow.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Builds BlackWidow repositories.
    /// </summary>
    internal class BlackWidowRepositoryConfigurator : IBlackWidowRepositoryConfigurator
    {
        public IGrabberRepository Repository { get; private set; }

        public IBlackWidowRepositoryConfigurator Use(IGrabberRepository repository)
        {
            Repository?.Dispose();
            Repository = repository;
            return this;
        }
    }
}
