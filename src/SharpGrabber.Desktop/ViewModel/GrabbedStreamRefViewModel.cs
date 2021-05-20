using DotNetTools.SharpGrabber.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
