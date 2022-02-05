using DotNetTools.SharpGrabber.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Builds <see cref="IGrabberAuthenticationService"/>.
    /// </summary>
    public interface IGrabberAuthenticationServiceBuilder
    {
        /// <summary>
        /// Configures to use <paramref name="store"/>.
        /// </summary>
        IGrabberAuthenticationServiceBuilder UseStore(IGrabberAuthenticationStore store);

        /// <summary>
        /// Builds the authentication service.
        /// </summary>
        IGrabberAuthenticationService Build();
    }

    public static class GrabberAuthenticationServiceBuilderExtensions
    {
        /// <summary>
        /// Configures to use the in-memory store.
        /// </summary>
        public static IGrabberAuthenticationServiceBuilder UseInMemoryStore(this IGrabberAuthenticationServiceBuilder builder)
        {
            return builder.UseStore(new InMemoryAuthenticationStore());
        }

        /// <summary>
        /// Configures to use the file store.
        /// </summary>
        public static IGrabberAuthenticationServiceBuilder UseFileStore(this IGrabberAuthenticationServiceBuilder builder, string fileName)
        {
            return builder.UseStore(new FileAuthenticationStore(fileName));
        }
    }
}
