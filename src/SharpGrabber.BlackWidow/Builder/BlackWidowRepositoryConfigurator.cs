using DotNetTools.SharpGrabber.BlackWidow.Repository;

namespace DotNetTools.SharpGrabber.BlackWidow
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
