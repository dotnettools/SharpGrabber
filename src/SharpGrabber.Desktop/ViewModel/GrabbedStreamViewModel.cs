using DotNetTools.SharpGrabber.Grabbed;

namespace SharpGrabber.Desktop.ViewModel
{
    public class GrabbedStreamViewModel : BaseViewModel
    {
        public GrabbedStreamViewModel(GrabbedHlsStreamMetadata stream)
        {
            Stream = stream;
            Name = stream.Name;
            Consideration = stream.Resolution?.ToString();
        }

        public GrabbedHlsStreamMetadata Stream { get; }

        public string Consideration { get; } = string.Empty;
    }
}
