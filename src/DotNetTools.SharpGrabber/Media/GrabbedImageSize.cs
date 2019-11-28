namespace DotNetTools.SharpGrabber.Media
{
    /// <summary>
    /// Stores grabbed image size
    /// </summary>
    public class GrabbedImageSize
    {
        #region Properties
        public int Width { get; }

        public int Height { get; }
        #endregion

        #region Constructor
        public GrabbedImageSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
        #endregion
    }
}