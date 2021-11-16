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
            var ownDic = feed.GetScripts().ToDictionary(s => s.Id);
            var otherDic = otherFeed.GetScripts().ToDictionary(s => s.Id);

            // return what's new
            foreach (var id in ownDic.Keys.Where(k => !otherDic.ContainsKey(k)))
                yield return new GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType.ScriptAdded, ownDic[id], null);

            // return what's removed
            foreach (var id in otherDic.Keys.Where(k => !ownDic.ContainsKey(k)))
                yield return new GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType.ScriptRemoved, null, otherDic[id]);

            // return what's changed
            foreach (var id in otherDic.Keys.Intersect(ownDic.Keys))
            {
                var ownScript = ownDic[id];
                var otherScript = otherDic[id];
                if (otherScript.Equals(ownScript))
                    continue;
                yield return new GrabberRepositoryFeedDifference(GrabberRepositoryFeedDifferenceType.ScriptChanged, ownScript, otherScript);
            }
        }
    }
}
