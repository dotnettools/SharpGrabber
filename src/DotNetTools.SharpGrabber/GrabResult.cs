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

        /// <summary>
        /// Optional date and time when this content was created
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Optional statistics grabbed from the page
        /// </summary>
        public GrabStatisticInfo Statistics { get; set; }

        /// <summary>
        /// Whether or not the media is secured. For example, <see cref="IsSecure"/> is true if a YouTube video
        /// has signature and its download links need to be deciphered.
        /// </summary>
        public bool IsSecure { get; set; }
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