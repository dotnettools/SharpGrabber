using System;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class ApiMimeContext
    {
        private readonly IMimeService _mimeService;

        public ApiMimeContext(IMimeService mimeService)
        {
            _mimeService = mimeService;
        }

        public string GetExtension(string mime)
        {
            return _mimeService.ExtractMimeExtension(mime);
        }
    }
}