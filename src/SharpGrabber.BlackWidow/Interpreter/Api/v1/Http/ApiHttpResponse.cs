using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Http
{
    public class ApiHttpResponse
    {
        public ApiHttpResponse(HttpResponseMessage response, string bodyText)
        {
            Success = response.IsSuccessStatusCode;
            StatusCode = (int)response.StatusCode;
            StatusText = response.ReasonPhrase;
            Headers = new Dictionary<string, string[]>();
            foreach (var header in response.Headers)
                Headers[header.Key] = header.Value.ToArray();
            BodyText = bodyText;
        }

        public int StatusCode { get; }

        public string StatusText { get; }

        public IDictionary<string, string[]> Headers { get; }

        public string BodyText { get; }

        public bool Success { get; }

        public void AssertSuccess()
        {
            if (!Success)
                throw new HttpRequestException($"The status code {StatusCode} does not indicate success.");
        }
    }
}
