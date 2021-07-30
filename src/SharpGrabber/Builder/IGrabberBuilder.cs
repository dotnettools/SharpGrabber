using System;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Builds a multi-purpose grabber from multiple single <see cref="IGrabber"/>s.
    /// </summary>
    public interface IGrabberBuilder
    {
        /// <summary>
        /// Makes the grabber use the specified <paramref name="services"/>.
        /// </summary>
        IGrabberBuilder UseServices(IGrabberServices services);

        /// <summary>
        /// Includes a grabber.
        /// </summary>
        IGrabberBuilder Add(IGrabber grabber);

        /// <summary>
        /// Includes a grabber by instantiating its type with parameterless constructor.
        /// </summary>
        IGrabberBuilder Add<T>() where T : IGrabber;

        /// <summary>
        /// Builds the final grabber.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the configuration of the builder is invalid.</exception>
        IGrabber Build();
    }
}
