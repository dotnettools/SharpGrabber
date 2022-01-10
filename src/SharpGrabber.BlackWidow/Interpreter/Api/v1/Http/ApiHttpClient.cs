using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Http
{
    public class ApiHttpClient
    {
        private HttpClient _client;

        public ApiHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiHttpResponse> SendAsync(ApiHttpRequest request)
        {
            using var message = await CreateRequestMessageAsync(request).ConfigureAwait(false);
            using var response = await _client.SendAsync(message).ConfigureAwait(false);
            return await ProcessResponseAsync(response, request).ConfigureAwait(false);
        }

        public Task<ApiHttpResponse> GetAsync(ApiHttpRequest request)
        {
            request.Method = HttpMethod.Get.Method;
            return SendAsync(request);
        }

        public Task<ApiHttpResponse> HeadAsync(ApiHttpRequest request)
        {
            request.Method = HttpMethod.Head.Method;
            return SendAsync(request);
        }

        public Task<ApiHttpResponse> PostAsync(ApiHttpRequest request)
        {
            request.Method = HttpMethod.Post.Method;
            return SendAsync(request);
        }

        public ApiHttpResponse Send(ApiHttpRequest request)
            => SendAsync(request).GetAwaiter().GetResult();

        public ApiHttpResponse Get(ApiHttpRequest request)
            => GetAsync(request).GetAwaiter().GetResult();

        public ApiHttpResponse Head(ApiHttpRequest request)
            => HeadAsync(request).GetAwaiter().GetResult();

        public ApiHttpResponse Post(ApiHttpRequest request)
            => PostAsync(request).GetAwaiter().GetResult();

        private Task<HttpRequestMessage> CreateRequestMessageAsync(ApiHttpRequest request)
        {
            var message = new HttpRequestMessage(new HttpMethod(request.Method), request.Url);

            foreach (var header in request.Headers)
                message.Headers.TryAddWithoutValidation(header.Key, header.Value);

            if (!string.IsNullOrEmpty(request.BodyText?.ToString()))
            {
                message.Content = new StringContent(request.BodyText.ToString());
            }

            return Task.FromResult(message);
        }

        private async Task<ApiHttpResponse> ProcessResponseAsync(HttpResponseMessage response,
            ApiHttpRequest request)
        {
            var contentType = response.Content.Headers.GetValues("Content-Type")?.FirstOrDefault();

            string bodyText = null;
            if (response.IsSuccessStatusCode)
            {
                var expectText = request.ExpectText;
                if (!expectText)
                    expectText = !string.IsNullOrEmpty(contentType) &&
                                 contentType.StartsWith("text/", StringComparison.InvariantCultureIgnoreCase);
                if (expectText)
                {
                    bodyText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            return new ApiHttpResponse(response, bodyText);
        }
    }
}