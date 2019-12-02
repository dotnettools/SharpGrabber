namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Describes size of a rectangle - width, and height.
    /// </summary>
    public class Size
    {
        #region Properties
        /// <summary>
        /// Size width
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Size height
        /// </summary>
        public int Height { get; }
        #endregion

        #region Constructor
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region Methods
        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
        #endregion
    }
}