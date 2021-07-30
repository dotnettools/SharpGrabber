using System;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Base implementation for <see cref="IGrabbedResource"/>
    /// </summary>
    public abstract class GrabbedResourceBase : IGrabbedResource
    {
        public GrabbedResourceBase(Uri resourceUri)
        {
            ResourceUri = resourceUri;
        }

        public Uri ResourceUri { get; }
    }
}
