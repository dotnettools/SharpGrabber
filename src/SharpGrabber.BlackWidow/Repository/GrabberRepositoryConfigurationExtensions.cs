using DotNetTools.SharpGrabber.BlackWidow.Builder;
using DotNetTools.SharpGrabber.BlackWidow.Repository.GitHub;
using DotNetTools.SharpGrabber.BlackWidow.Repository.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Defines extension methods for <see cref="IBlackWidowRepositoryConfigurator"/>
    /// </summary>
    public static class GrabberRepositoryConfigurationExtensions
    {
        /// <summary>
        /// Configures to use a physical repository.
        /// </summary>
        public static IBlackWidowRepositoryConfigurator UseMemory(this IBlackWidowRepositoryConfigurator configurator, bool readOnly = false)
        {
            var repo = new InMemoryRepository(readOnly);
            return configurator.Use(repo);
        }

        /// <summary>
        /// Configures to use a physical repository.
        /// </summary>
        public static IBlackWidowRepositoryConfigurator UsePhysical(this IBlackWidowRepositoryConfigurator configurator, string rootPath, bool readOnly = false)
        {
            var repo = new PhysicalGrabberRepository(rootPath, readOnly);
            return configurator.Use(repo);
        }

        /// <summary>
        /// Configures to use a GitHub repository.
        /// </summary>
        public static IBlackWidowRepositoryConfigurator UseGitHub(this IBlackWidowRepositoryConfigurator configurator, Action<GitHubGrabberRepository> configure)
        {
            var repo = new GitHubGrabberRepository();
            configure.Invoke(repo);
            return configurator.Use(repo);
        }

        /// <summary>
        /// Configures to use the official GitHub repository.
        /// </summary>
        public static IBlackWidowRepositoryConfigurator UseOfficial(this IBlackWidowRepositoryConfigurator configurator)
        {
            var repo = new OfficialGrabberRepository();
            return configurator.Use(repo);
        }
    }
}
