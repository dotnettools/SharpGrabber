namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Represents common work status types.
    /// </summary>
    internal enum WorkStatusType
    {
        /// <summary>
        /// The work has just started
        /// </summary>
        Initiating,

        /// <summary>
        /// The grabber is downloading a page - usually the original uri.
        /// </summary>
        DownloadingPage,

        /// <summary>
        /// The grabber is downloading a file.
        /// </summary>
        DownloadingFile,

        /// <summary>
        /// The grabber is resolving one or more URIs.
        /// </summary>
        ResolvingUri,

        /// <summary>
        /// The grabber is decrypting/deciphering one or more URIs.
        /// </summary>
        Decrypting,

        /// <summary>
        /// Indicates that type of the status is uncommon and a description is provided.
        /// </summary>
        Other,
    };

    /// <summary>
    /// Provides status of a task.
    /// </summary>
    internal class WorkStatus
    {

        public WorkStatus()
        {
        }

        public WorkStatus(double? progress, string description, WorkStatusType type = WorkStatusType.Other)
        {
            Update(progress, description, type);
        }


        /// <summary>
        /// Work type
        /// </summary>
        public WorkStatusType Type { get; set; } = WorkStatusType.Initiating;

        /// <summary>
        /// Work progress
        /// </summary>
        /// <remarks>If work progress is undetermined, value of this property must be NULL.</remarks>
        public double? Progress { get; set; }

        /// <summary>
        /// Description of the status - e.g. "Downloading target url..."
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Updates properties of the status with a single call.
        /// </summary>
        public void Update(double? progress, string description, WorkStatusType type = WorkStatusType.Other)
        {
            Progress = progress;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Updates properties of the status with a single call.
        /// </summary>
        public void Update(double? progress, WorkStatusType type)
        {
            Progress = progress;
            Type = type;
        }
    }
}