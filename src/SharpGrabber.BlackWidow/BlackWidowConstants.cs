using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow
{
    /// <summary>
    /// Defines BlackWidow-related constants.
    /// </summary>
    public static class BlackWidowConstants
    {
        public static class GitHub
        {
            public static class OfficialRepository
            {
                /// <summary>
                /// The offical repository name
                /// </summary>
                public const string RepositoryAddress = "dotnettools/SharpGrabber";

                /// <summary>
                /// Name of the main branch
                /// </summary>
                public const string MasterBranch = "master";

                /// <summary>
                /// Path to the directory that contains the feed file and the scripts
                /// </summary>
                public const string RootPath = "blackwidow/repo";

                /// <summary>
                /// Name of the feed JSON file
                /// </summary>
                public const string FeedFileName = "feed.json";
            }
        }
    }
}
