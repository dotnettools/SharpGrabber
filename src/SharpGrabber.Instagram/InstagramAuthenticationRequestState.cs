using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Instagram
{
    /// <summary>
    /// Describes the state object for an Instagram authentication request.
    /// </summary>
    public sealed class InstagramAuthenticationRequestState
    {
        public InstagramAuthenticationRequestState(IInstaApi api)
        {
            Api = api;
        }

        /// <summary>
        /// Gets the Instagram API client.
        /// </summary>
        public IInstaApi Api { get; }
    }
}
