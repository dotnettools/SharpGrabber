using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Auth
{
    /// <summary>
    /// Provides a store for authentication information.
    /// </summary>
    public interface IGrabberAuthenticationStore
    {
        /// <summary>
        /// Gets a value by <paramref name="key"/>.
        /// </summary>
        string Get(string key, string @default = null);

        /// <summary>
        /// Sets the entry defined by <paramref name="key"/> to <paramref name="value"/>.
        /// </summary>
        void Set(string key, string value);

        /// <summary>
        /// Deletes an entry by <paramref name="key"/> if it exists.
        /// </summary>
        void Delete(string key);
    }
}
