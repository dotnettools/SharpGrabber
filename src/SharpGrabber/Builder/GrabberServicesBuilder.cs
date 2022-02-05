using DotNetTools.SharpGrabber.Auth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Builds <see cref="IGrabberServices"/>.
    /// </summary>
    public class GrabberServicesBuilder : IGrabberServicesBuilder
    {
        private IGrabberAuthenticationService _authenticationService;
        private Func<HttpClient> _httpClientProvider;
        private IMimeService _mimeService;

        private GrabberServicesBuilder() { }

        /// <summary>
        /// Creates a new builder.
        /// </summary>
        public static IGrabberServicesBuilder New()
            => new GrabberServicesBuilder();

        public IGrabberServicesBuilder UseAuthenticationService(IGrabberAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            return this;
        }

        public IGrabberServicesBuilder UseHttpClientProvider(Func<HttpClient> provider)
        {
            _httpClientProvider = provider;
            return this;
        }

        public IGrabberServicesBuilder UseMimeService(IMimeService mimeService)
        {
            _mimeService = mimeService;
            return this;
        }

        public IGrabberServices Build()
        {
            return new GrabberServices(_httpClientProvider, _mimeService, _authenticationService);
        }
    }
}
