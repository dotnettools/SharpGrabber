using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
