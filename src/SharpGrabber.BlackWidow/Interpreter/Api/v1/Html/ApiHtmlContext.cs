using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Html
{
    public class ApiHtmlContext
    {
        public ApiHtmlElement Parse(string source)
        {
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(source);
            return new ApiHtmlElement(doc.DocumentElement);
        }

        public string Encode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        public string Decode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }
    }
}
