using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Http
{
    public class ApiHttpRequest
    {
        public string Url { get; set; }

        public string Method { get; set; }

        public object BodyText { get; set; }

        public bool ExpectText { get; set; }

        public Dictionary<string, IList<string>> Headers { get; set; } = new Dictionary<string, IList<string>>();

        public ApiHttpRequest AddHeader(string name, string value)
        {
            if (!Headers.TryGetValue(name, out var list))
                Headers.Add(name, list = new List<string>());
            list.Add(value);
            return this;
        }

        public ApiHttpRequest SetHeader(string name, string value)
        {
            if (!Headers.TryGetValue(name, out var list))
                Headers.Add(name, list = new List<string>());
            list.Clear();
            list.Add(value);
            return this;
        }
    }
}