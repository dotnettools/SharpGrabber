using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Result of a grab request - <see cref="GrabResult"/> is returned by grabbers containing grabbed resources and
    /// information obtained from the page.
    /// </summary>
    public class GrabResult
    {
        #region Properties
        public Uri OriginalUri { get; }

        /// <summary>
        /// List of grabbed resources
        /// </summary>
        public IList<IGrabbed> Resources { get; }

        /// <summary>
        /// Subject of the page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the content
        /// </summary>
        public string Description { get; set; }
        #endregion

        #region Constructors
        public GrabResult(Uri originalUri)
        {
            Resources = new List<IGrabbed>();
            OriginalUri = originalUri;
        }

        public GrabResult(Uri originalUri, IList<IGrabbed> grabbedList)
        {
            Resources = grabbedList;
            OriginalUri = originalUri;
        }
        #endregion
    }
}
