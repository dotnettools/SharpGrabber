using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Default implementation for <see cref="IMimeService"/>.
    /// </summary>
    public sealed class DefaultMimeService : IMimeService
    {
        private static readonly Regex _mimeRegex = new(@"^[A-Za-z0-9\-_]+/([A-Za-z0-9\-_]+)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static DefaultMimeService _instance;

        public static DefaultMimeService Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (typeof(DefaultMimeService))
                    if (_instance != null)
                        return _instance;
                    else
                        return _instance = new DefaultMimeService();
            }
        }

        private DefaultMimeService()
        {
            if (_instance != null)
                throw new InvalidOperationException($"{typeof(DefaultMimeService)} is singleton and cannot be constructed twice.");
            _instance = this;
        }

        public string ExtractMimeExtension(string mime)
        {
            var mimeInfo = MimeType.FindMime(mime);
            if (mimeInfo != null)
                return mimeInfo.Types.First();

            var match = _mimeRegex.Match(mime);
            if (!match.Success)
                return null;

            return match.Groups[1].Value;
        }

        public bool TryGetMimeByExtension(string extension, out string mime)
        {
            var mimeInfo = MimeType.FindMimesByExtension(extension).FirstOrDefault();
            if (mimeInfo == null)
            {
                mime = null;
                return false;
            }
            mime = mimeInfo.Mime;
            return true;
        }
    }
}
