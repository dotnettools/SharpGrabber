using DotNetTools.SharpGrabber.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Configures and builds <see cref="IGrabberServices"/>.
    /// </summary>
    public interface IGrabberServicesBuilder
    {
        /// <summary>
        /// Configures the grabber services to use <paramref name="provider"/>.
        /// </summary>
        IGrabberServicesBuilder UseHttpClientProvider(Func<HttpClient> provider);

        /// <summary>
        /// Configures the grabber services to use <paramref name="mimeService"/>.
        /// </summary>
        IGrabberServicesBuilder UseMimeService(IMimeService mimeService);

        /// <summary>
        /// Configures the grabber services to use <paramref name="authenticationService"/>.
        /// </summary>
        IGrabberServicesBuilder UseAuthenticationService(IGrabberAuthenticationService authenticationService);

        /// <summary>
        /// Builds the grabber services object.
        /// </summary>
        IGrabberServices Build();
    }
}
