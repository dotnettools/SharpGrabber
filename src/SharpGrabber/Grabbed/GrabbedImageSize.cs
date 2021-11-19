namespace DotNetTools.SharpGrabber.Grabbed
{
    /// <summary>
    /// Stores grabbed image size
    /// </summary>
    public class GrabbedImageSize : RectSize
    {
        public GrabbedImageSize()
        {
        }

        public GrabbedImageSize(int width, int height) : base(width, height)
        {
        }
    }
}