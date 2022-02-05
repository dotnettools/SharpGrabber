using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using DotNetTools.SharpGrabber.Auth;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Internal;

namespace DotNetTools.SharpGrabber
{
    public class GrabberServices : IGrabberServices
    {
        /// <summary>
        /// The built-in instance of <see cref="GrabberServices"/>
        /// </summary>
        public static readonly GrabberServices Default = new();

        private readonly Func<HttpClient> _httpClientProvider;

        public GrabberServices(Func<HttpClient> httpClientProvider = null, IMimeService mime = null,
            IGrabberAuthenticationService authService = null)
        {
            _httpClientProvider = httpClientProvider ?? GetGlobalHttpClient;
            Mime = mime ?? DefaultMimeService.Instance;
            Authentication = authService ?? new GrabberAuthenticationService();
        }
        public IMimeService Mime { get; }

        public IGrabberAuthenticationService Authentication { get; }

        public HttpClient GetClient()
            => _httpClientProvider();

        private static HttpClient _globalHttpClient;

        private static HttpClient GetGlobalHttpClient()
        {
            if (_globalHttpClient == null)
                lock (typeof(GrabberBase))
                {
                    if (_globalHttpClient == null)
                    {
                        var defaultProvider = new DefaultGlobalHttpProvider();
                        _globalHttpClient = defaultProvider.GetClient();
                    }
                }
            return _globalHttpClient;
        }
    }
}
