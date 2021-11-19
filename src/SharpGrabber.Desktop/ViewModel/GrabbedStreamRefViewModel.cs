using DotNetTools.SharpGrabber.Grabbed;

namespace SharpGrabber.Desktop.ViewModel
{
    public class GrabbedStreamRefViewModel : BaseViewModel
    {
        public GrabbedStreamRefViewModel(GrabbedHlsStreamReference streamRef)
        {
            Reference = streamRef;
            Name = streamRef.Resolution ?? "Master Playlist";
        }

        public GrabbedHlsStreamReference Reference { get; }

        public string Consideration { get; set; } = "M3U8 File";
    }
}
