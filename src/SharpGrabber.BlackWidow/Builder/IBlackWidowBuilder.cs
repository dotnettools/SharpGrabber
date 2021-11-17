﻿using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
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
        IBlackWidowBuilder ConfigureLocalRepository(Action<IBlackWidowRepositoryConfigurator> configure);

        /// <summary>
        /// Configures the remote repository.
        /// </summary>
        IBlackWidowBuilder ConfigureRemoteRepository(Action<IBlackWidowRepositoryConfigurator> configure);

        /// <summary>
        /// Sets the script host.
        /// </summary>
        IBlackWidowBuilder SetScriptHost(IScriptHost scriptHost);

        /// <summary>
        /// Sets the change detector.
        /// </summary>
        IBlackWidowBuilder SetChangeDetector(IGrabberRepositoryChangeDetector changeDetector);

        /// <summary>
        /// Sets <paramref name="interpreterService"/> to be used.
        /// </summary>
        IBlackWidowBuilder UseInterpreterService(IGrabberScriptInterpreterService interpreterService);

        /// <summary>
        /// Configures the interpreter service.
        /// </summary>
        IBlackWidowBuilder ConfigureInterpreterService(Action<IGrabberScriptInterpreterServiceConfigurator> configure);

        /// <summary>
        /// Builds the service.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown in case of misconfiguration.</exception>
        Task<IBlackWidowService> BuildAsync();
    }
}
