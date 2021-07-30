namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Represents format of media e.g. mp4
    /// </summary>
    public class MediaFormat
    {
        #region Properties
        /// <summary>
        /// Mime of the media e.g. video/mp4, image/jpeg
        /// </summary>
        public string Mime { get; set; }

        /// <summary>
        /// Suggested extension for the file e.g. mp4, mp3 etc.
        /// </summary>
        public string Extension { get; set; }
        #endregion

        #region Methods

        public MediaFormat() { }

        public MediaFormat(string mime, string extension)
        {
            Mime = mime;
            Extension = extension;
        }

        #endregion
    }
}
