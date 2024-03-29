﻿using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Html;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Http;
using DotNetTools.SharpGrabber.Exceptions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class ApiHostObject
    {
        public ApiHostObject(IGrabberServices grabberServices)
        {
            Http = new ApiHttpContext(grabberServices);
            Mime = new ApiMimeContext(grabberServices.Mime);
        }

        public ApiGrabberContext Grabber { get; } = new ApiGrabberContext();

        public ApiHttpContext Http { get; }

        public ApiHtmlContext Html { get; } = new ApiHtmlContext();

        public ApiMimeContext Mime { get; }
    }
}