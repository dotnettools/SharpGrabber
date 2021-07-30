namespace DotNetTools.SharpGrabber
{
    /// <summary>
    /// Describes size of a rectangle - width, and height.
    /// </summary>
    public class RectSize
    {
        public RectSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Size width
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Size height
        /// </summary>
        public int Height { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}