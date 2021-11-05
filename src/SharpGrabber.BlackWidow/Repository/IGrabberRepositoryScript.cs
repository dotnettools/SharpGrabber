using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// Gets the friendly name of this script.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the semantic version.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the script type, which is used to understand how the script should be interpreted.
        /// </summary>
        GrabberScriptType Type { get; }

        /// <summary>
        /// Gets the BlackWidow API version.
        /// </summary>
        int ApiVersion { get; }

        /// <summary>
        /// Gets whether the script is deprecated or works flawlessly.
        /// </summary>
        bool IsDeprecated { get; }

        /// <summary>
        /// Gets a list of optional regular expressions at least one of which a URI must match before grabbing.
        /// </summary>
        string[] SupportedRegularExpressions { get; }

        /// <summary>
        /// Tests whether or not <paramref name="uri"/> matches any of <see cref="SupportedRegularExpressions"/>.
        /// </summary>
        bool IsMatch(Uri uri);
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
