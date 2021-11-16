using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Describes the state of a repository.
    /// </summary>
    public interface IGrabberRepositoryFeed
    {
        /// <summary>
        /// Tries to find a script with the specified identifier in the repository.
        /// </summary>
        /// <returns>The script if found; otherwise NULL.</returns>
        IGrabberRepositoryScript GetScript(string scriptId);

        /// <summary>
        /// Enumerates descriptors of all scripts in the repository.
        /// </summary>
        IEnumerable<IGrabberRepositoryScript> GetScripts();
    }

    /// <summary>
    /// Defines extension methods for <see cref="IGrabberRepositoryFeed"/>.
    /// </summary>
    public static class GrabberRepositoryFeedExtensions
    {
        /// <summary>
        /// Gets all the differences between this feed and <paramref name="otherFeed"/>.
        /// </summary>
        public static IEnumerable<GrabberRepositoryFeedDifference> GetDifferences(this IGrabberRepositoryFeed feed, IGrabberRepositoryFeed otherFeed)
        {
            var dic1 = otherFeed.GetScripts().ToDictionary(s => s.Id);
            var dic2 = feed.GetScripts().ToDictionary(s => s.Id);

            // return what's new
            foreach (var id in dic2.Keys.Where(k => !dic1.ContainsKey(k)))
                yield return new GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType.ScriptAdded, null, dic2[id]);

            // return what's removed
            foreach (var id in dic1.Keys.Where(k => !dic2.ContainsKey(k)))
                yield return new GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType.ScriptRemoved, dic1[id], null);

            // return what's changed
            foreach (var id in dic1.Keys.Intersect(dic2.Keys))
            {
                var ownScript = dic1[id];
                var otherScript = dic2[id];
                if (ownScript.Equals(otherScript))
                    continue;
                yield return new GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType.ScriptChanged, ownScript, otherScript);
            }
        }
    }
}
