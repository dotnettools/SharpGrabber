using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetTools.SharpGrabber.Internal
{
    /// <summary>
    /// Contains mime-related helper methods.
    /// </summary>
    internal static class MimeHelper
    {
        #region Constants
        private static readonly Regex _mimeRegex = new Regex(@"^[A-Za-z0-9\-_]+/([A-Za-z0-9\-_]+)", RegexOptions.Compiled | RegexOptions.Singleline);
        #endregion

        #region Methods
        public static string ExtractMimeExtension(string mime)
        {
            var mimeInfo = MimeType.FindMime(mime);
            if (mimeInfo != null)
                return mimeInfo.Types.First();

            var match = _mimeRegex.Match(mime);
            if (!match.Success)
                return null;

            return match.Groups[1].Value;
        }
        #endregion
    }
}
