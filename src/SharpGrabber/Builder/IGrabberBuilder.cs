using System;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Builds a multi-purpose grabber from multiple single <see cref="IGrabber"/>s.
    /// </summary>
    public interface IGrabberBuilder
    {
        /// <summary>
        /// Configures the grabber to use the specified <paramref name="services"/>.
        /// </summary>
        IGrabberBuilder UseServices(IGrabberServices services);

        /// <summary>
        /// Includes a grabber.
        /// </summary>
        IGrabberBuilder Add(IGrabber grabber);

        /// <summary>
        /// Includes a grabber by instantiating its type.
        /// </summary>
        IGrabberBuilder Add<T>() where T : IGrabber;

        /// <summary>
        /// Builds the final grabber.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the configuration of the builder is invalid.</exception>
        IMultiGrabber Build();
    }

    /// <summary>
    /// Provides extension methods for <see cref="IGrabberBuilder"/>.
    /// </summary>
    public static class GrabberBuilderExtensions
    {
        /// <summary>
        /// Configures the grabber to use the default services.
        /// </summary>
        public static IGrabberBuilder UseDefaultServices(this IGrabberBuilder builder)
        {
            return builder.UseServices(GrabberServices.Default);
        }
    }
}
