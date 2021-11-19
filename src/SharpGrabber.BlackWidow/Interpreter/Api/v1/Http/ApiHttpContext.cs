using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1.Http
{
    public class ApiHttpContext
    {
        private IGrabberServices _grabberServices;

        public ApiHttpContext(IGrabberServices grabberServices)
        {
            _grabberServices = grabberServices;
            var client = _grabberServices.GetClient();
            Client = new ApiHttpClient(client);
        }

        public ApiHttpClient Client { get; }
    }
}
