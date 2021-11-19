using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Defines all possible types of difference between two <see cref="IGrabberRepositoryFeed"/> objects.
    /// </summary>
    public enum GrabberRepositoryFeedDifferenceType
    {
        /// <summary>
        /// Indicates that a script has been added.
        /// </summary>
        ScriptAdded,

        /// <summary>
        /// Indicates that a script has been removed.
        /// </summary>
        ScriptRemoved,

        /// <summary>
        /// Indicates that a script has been changed.
        /// </summary>
        ScriptChanged
    }

    /// <summary>
    /// Describes a difference between two <see cref="IGrabberRepositoryFeed"/> objects.
    /// </summary>
    public class GrabberRepositoryFeedDifference
    {
        public GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType type, IGrabberRepositoryScript ownScript, IGrabberRepositoryScript otherScript)
        {
            Type = type;
            OwnScript = ownScript;
            OtherScript = otherScript;
        }

        /// <summary>
        /// Gets the type of this difference.
        /// </summary>
        public GrabberRepositoryFeedDifferenceType Type { get; }

        /// <summary>
        /// Gets the descriptor for the own script.
        /// This value is NULL if <see cref="Type"/> is <see cref="GrabberRepositoryFeedDifferenceType.ScriptAdded"/>.
        /// </summary>
        public IGrabberRepositoryScript OwnScript { get; }

        /// <summary>
        /// Gets the descriptor for the other script.
        /// This value is NULL if <see cref="Type"/> is <see cref="GrabberRepositoryFeedDifferenceType.ScriptRemoved"/>.
        /// </summary>
        public IGrabberRepositoryScript OtherScript { get; }
    }
}
