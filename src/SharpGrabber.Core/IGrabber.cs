using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Grabs useful resources from certain URLs which may refer to a certain website,
    /// or files with specific types.
    /// </summary>
    /// <remarks>
    /// An instance of this type is thread-safe and its members can be accessed simultaneously.
    /// </remarks>
    public interface IGrabber
    {
        /// <summary>
        /// Gets the name of the grabber e.g. YouTube.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the default grab options for this grabber.
        /// </summary>
        GrabOptions DefaultGrabOptions { get; }

        /// <summary>
        /// Gets the reference to the grabber services.
        /// </summary>
        IGrabberServices Services { get; }

        /// <summary>
        /// Briefly checks if this grabber supports the specified URI.
        /// </summary>
        bool Supports(Uri uri);

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources regarding the specified <paramref name="options"/>,
        /// optionally reporting <paramref name="progress"/>.
        /// In case of unsupported URI, NULL should be returned.
        /// </summary>
        /// <param name="uri">The target URI</param>
        /// <param name="cancellationToken">The cancellation token that may cancel the grab operation</param>
        /// <param name="options">Grab options, or NULL if not necessary</param>
        /// <param name="progress">The object for progress report, or NULL if not necessary</param>
        Task<GrabResult> GrabAsync(Uri uri, CancellationToken cancellationToken, GrabOptions options, IProgress<double> progress);
    }

    /// <summary>
    /// Provides extension methods for <see cref="IGrabber"/>.
    /// </summary>
    public static class GrabberExtensions
    {
        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources. In case of unsupported URI, NULL should
        /// be returned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<GrabResult> GrabAsync(this IGrabber grabber, Uri uri)
        {
            return grabber.GrabAsync(uri, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources. In case of unsupported URI, NULL should
        /// be returned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<GrabResult> GrabAsync(this IGrabber grabber, Uri uri, CancellationToken cancellationToken)
        {
            return grabber.GrabAsync(uri, cancellationToken, grabber.DefaultGrabOptions);
        }

        /// <summary>
        /// Asynchronously looks up the specified URI and grabs useful resources regarding the specified <paramref name="options"/>.
        /// In case of unsupported URI, NULL should be returned.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<GrabResult> GrabAsync(this IGrabber grabber, Uri uri, CancellationToken cancellationToken, GrabOptions options)
        {
            return grabber.GrabAsync(uri, cancellationToken, options, null);
        }

    }
}
