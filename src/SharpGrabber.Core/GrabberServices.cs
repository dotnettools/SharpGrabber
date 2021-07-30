using System;
using System.Net.Http;

namespace DotNetTools.SharpGrabber
{
    public class GrabberServices : IGrabberServices
    {
        private readonly Func<HttpClient> _httpClientProvider;
        public static readonly GrabberServices Default = new();

        public GrabberServices(Func<HttpClient> httpClientProvider = null, IMimeService mime = null)
        {
            _httpClientProvider = httpClientProvider ?? GetGlobalHttpClient;
            Mime = mime ?? DefaultMimeService.Instance;
        }
        public IMimeService Mime { get; }

        public HttpClient GetClient()
            => _httpClientProvider();

        private static HttpClient _globalHttpClient;

        private static HttpClient GetGlobalHttpClient()
        {
            if (_globalHttpClient == null)
                lock (typeof(GrabberBase))
                {
                    if (_globalHttpClient == null)
                        _globalHttpClient = new HttpClient();
                }
            return _globalHttpClient;
        }
    }
}
