using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Describes a script in a BlackWidow repository.
    /// </summary>
    public interface IGrabberRepositoryScript
    {
        /// <summary>
        /// Gets the string that uniquely identifies this script.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the script type, which is used to understand how the script should be interpreted.
        /// </summary>
        GrabberScriptType Type { get; }

        /// <summary>
        /// Gets the semantic version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets whether the script is deprecated or works flawlessly.
        /// </summary>
        bool IsDeprecated { get; }
    }

    /// <summary>
    /// Defines extension methods for <see cref="IGrabberRepositoryScript"/>.
    /// </summary>
    public static class GrabberRepositoryScriptExtensions
    {
        /// <summary>
        /// Returns version of the script as <see cref="System.Version"/>.
        /// </summary>
        public static Version GetVersion(this IGrabberRepositoryScript script)
            => string.IsNullOrEmpty(script.Version) ? null : Version.Parse(script.Version);
    }
}
