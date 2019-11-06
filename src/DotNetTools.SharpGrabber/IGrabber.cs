using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Where implemented, can grab useful resources from certain types of URI.
    /// Many <see cref="IGrabber"/> objects may be registered on a <see cref="MultiGrabber"/> allowing usage
    /// of all grabbers by a single call.
    /// </summary>
    /// <remarks>
    /// Methods of same instance of classes implementing <see cref="IGrabber"/> are not expected to be used
    /// simultaneously. Status of a grab is available through <see cref="IGrabber.Status"/>.
    /// </remarks>
    public interface IGrabber
    {
        /// <summary>
        /// Reports status and grab progress
        /// </summary>
        WorkStatus Status { get; }

        /// <summary>
        /// Default grab options for this grabber - if none is specified while invoking <see cref="IGrabber.Grab"/>.
        /// </summary>
        GrabOptions DefaultGrabOptions { get; }

        /// <summary>
        /// Gets an array of case-insensitive strings representing the supported schemes, such as "http".
        /// In case this grabber is not to be associated with certain protocols, NULL may be returned.
        /// </summary>
        string[] GetSupportedSchemes();

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources.
        /// </summary>
        Task<IEnumerable<IGrabbed>> Grab(Uri uri);

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources regarding the specified <paramref name="options"/>.
        /// </summary>
        Task<IEnumerable<IGrabbed>> Grab(Uri uri, GrabOptions options);
    }
}
