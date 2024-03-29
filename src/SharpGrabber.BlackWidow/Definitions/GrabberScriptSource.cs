﻿using System.IO;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Definitions
{
    /// <summary>
    /// Default implementation for <see cref="IGrabberScriptSource"/>
    /// </summary>
    public class GrabberScriptSource : IGrabberScriptSource
    {
        /// <summary>
        /// Refers to a static empty source.
        /// </summary>
        public static readonly GrabberScriptSource Empty = new(string.Empty);

        private readonly string _source;

        public GrabberScriptSource(string source)
        {
            _source = source;
        }

        /// <summary>
        /// Creates a <see cref="GrabberScriptSource"/> by reading all the source code from a file.
        /// </summary>
        public static GrabberScriptSource FromFile(string fileName)
        {
            var src = File.ReadAllText(fileName);
            return new GrabberScriptSource(src);
        }

        public string GetSource()
            => _source;

        public Task<string> GetSourceAsync()
            => Task.FromResult(_source);
    }
}
