using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Html
{
    public class ApiHtmlElement
    {
        private readonly IElement _element;
        private Dictionary<string, string> _attributes;

        public ApiHtmlElement(IElement element)
        {
            _element = element;
        }

        public string TagName => _element.TagName;

        public string InnerHTML => _element.InnerHtml;

        public string OuterHTML => _element.OuterHtml;

        public string InnerText => _element.TextContent;

        public int ChildrenCount => _element.Children.Length;

        public IDictionary<string, string> Attributes
        {
            get
            {
                if (_attributes != null)
                    return _attributes;

                _attributes = _element.Attributes.AsEnumerable().ToDictionary(k => k.Name, k => k.Value);
                return _attributes;
            }
        }

        public string GetAttribute(string name)
        {
            return Attributes.GetOrDefault(name);
        }

        public ApiHtmlElement ChildAt(int index)
        {
            return new ApiHtmlElement(_element.Children[index]);
        }

        public ApiHtmlElement Select(string cssSelector)
        {
            return new ApiHtmlElement(_element.QuerySelector(cssSelector));
        }

        public ApiHtmlElement[] SelectAll(string cssSelector)
        {
            return _element
                .QuerySelectorAll(cssSelector)
                .Select(n => new ApiHtmlElement(n))
                .ToArray();
        }
    }
}
