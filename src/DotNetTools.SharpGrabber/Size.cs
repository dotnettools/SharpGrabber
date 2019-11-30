namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Describes size of a rectangle - width, and height.
    /// </summary>
    public class Size
    {
        #region Properties
        public int Width { get; }

        public int Height { get; }
        #endregion

        #region Constructor
        public Size(int width, int height)
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