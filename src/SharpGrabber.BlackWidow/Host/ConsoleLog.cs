using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Host
{
    /// <summary>
    /// Describes a log entry.
    /// </summary>
    public class ConsoleLog
    {
        public ConsoleLog(ConsoleLogLevel level, params object[] objects)
        {
            Level = level;
            Objects = objects;
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public ConsoleLogLevel Level { get; }

        /// <summary>
        /// Gets the logged objects.
        /// </summary>
        public object[] Objects { get; }
    }
}
