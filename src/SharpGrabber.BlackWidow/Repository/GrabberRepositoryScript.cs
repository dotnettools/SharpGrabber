using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Default implementation for <see cref="IGrabberRepositoryScript"/>
    /// </summary>
    public class GrabberRepositoryScript : IGrabberRepositoryScript
    {
        public string Id { get; set; }

        public GrabberScriptType Type { get; set; }

        public string Version { get; set; }

        public bool IsDeprecated { get; set; }
    }
}
