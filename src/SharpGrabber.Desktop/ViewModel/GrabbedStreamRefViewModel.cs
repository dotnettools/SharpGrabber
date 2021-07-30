using DotNetTools.SharpGrabber.Grabbed;

namespace SharpGrabber.Desktop.ViewModel
{
    public class GrabbedStreamRefViewModel : BaseViewModel
    {
        public GrabbedStreamRefViewModel(GrabbedStreamReference streamRef)
        {
            Reference = streamRef;
            Name = streamRef.Resolution ?? "Master Playlist";
        }

        public GrabbedStreamReference Reference { get; }

        public string Consideration { get; set; } = "M3U8 File";
    }
}
