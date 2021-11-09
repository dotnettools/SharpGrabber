using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Builder
{
    /// <summary>
    /// Builds a <see cref="IBlackWidowService"/>.
    /// </summary>
    public interface IBlackWidowBuilder
    {
        /// <summary>
        /// Configures the local repository.
        /// </summary>
        IBlackWidowBuilder ConfigureLocalRepository(Action<IBlackWidowRepositoryBuilder> configurator);

        /// <summary>
        /// Configures the remote repository.
        /// </summary>
        IBlackWidowBuilder ConfigureRemoteRepository(Action<IBlackWidowRepositoryBuilder> configurator);

        /// <summary>
        /// Sets the script host.
        /// </summary>
        IBlackWidowBuilder SetScriptHost(IScriptHost scriptHost);

        /// <summary>
        /// Sets <paramref name="interpreterService"/> to be used.
        /// </summary>
        IBlackWidowBuilder UseInterpreterService(IGrabberScriptInterpreterService interpreterService);

        /// <summary>
        /// Builds the service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown in case of misconfiguration.</exception>
        Task<IBlackWidowService> BuildAsync();
    }
}
