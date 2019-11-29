using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Where implemented, can grab useful resources from certain types of URI.
    /// Many <see cref="IGrabber"/> objects may be registered on a <see cref="MultiGrabber"/> allowing usage
    /// of all grabbers by a single call.
    /// </summary>
    /// <remarks>
    /// Members of the same instance of <see cref="IGrabber"/> object are not expected to be used
    /// simultaneously. Status of an active grab is available through <see cref="IGrabber.Status"/>.
    /// </remarks>
    public interface IGrabber
    {
        /// <summary>
        /// Name of the grabber e.g. YouTube
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Reports status and grab progress
        /// </summary>
        WorkStatus Status { get; }

        /// <summary>
        /// Default grab options for this grabber - if none is specified while invoking <see cref="IGrabber.Grab"/>.
        /// </summary>
        GrabOptions DefaultGrabOptions { get; }
        
        /// <summary>
        /// Briefly checks if this grabber supports the specified URI.
        /// </summary>
        bool Supports(Uri uri);

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources. In case of unsupported URI, NULL should
        /// be returned.
        /// </summary>
        Task<GrabResult> GrabAsync(Uri uri);
        
        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources. In case of unsupported URI, NULL should
        /// be returned.
        /// </summary>
        Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources regarding the specified <paramref name="options"/>.
        /// In case of unsupported URI, NULL should be returned.
        /// </summary>
        Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options);
    }
}
