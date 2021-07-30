using System;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Describes a comment. All properties are optional.
    /// </summary>
    public class GrabbedComment
    {
        /// <summary>
        /// Gets or sets the name of the author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the URI for author's dedicated page.
        /// </summary>
        public Uri AuthorUri { get; set; }

        /// <summary>
        /// Gets or sets the URI for author's profile image thumbnail.
        /// </summary>
        public Uri AuthorThumbnailUri { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the publication date.
        /// </summary>
        public DateTime? PublishedAt { get; set; }

        /// <summary>
        /// Gets or sets the total number of likes.
        /// </summary>
        public int? LikeCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of dislikes.
        /// </summary>
        public int? DislikeCount { get; set; }

        /// <summary>
        /// Gets or sets the replies to the comment.
        /// May be NULL if not available, applicable, or implemented.
        /// </summary>
        public Lazy<Task<GrabbedComments>> Replies { get; set; }
    }
}
