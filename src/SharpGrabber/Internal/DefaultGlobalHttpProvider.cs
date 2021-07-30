using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DotNetTools.SharpGrabber.Internal
{
    /// <summary>
    /// Contains helper methods related to HTTP clients.
    /// </summary>
    internal class DefaultGlobalHttpProvider
    {
        /// <summary>
        /// Default user agent used for HTTP clients
        /// </summary>
        public static string DefaultUserAgent { get; set; } = "Mozilla/5.0 (Windows NT 5.1; rv:55.0) Gecko/20100101 Firefox/55.0";

        /// <summary>
        /// Creates and configures an <see cref="HttpClient"/>.
        /// </summary>
        public HttpClient GetClient()
        {
            var handler = new HttpClientHandler
            {
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
            return client;
        }
    }
}