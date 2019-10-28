using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Where implemented, can grab useful resources from certain types of URI.
    /// Many <see cref="IGrabber"/> objects may be registered on a <see cref="MultiGrabber"/> allowing usage
    /// of all grabbers bya single call.
    /// </summary>
    public interface IGrabber
    {
        /// <summary>
        /// Gets an array of case-insensitive strings representing the supported schemes, such as "http".
        /// In case this grabber is not to be associated with certain protocols, NULL may be returned.
        /// </summary>
        string[] GetSupportedSchemes();


    }
}
