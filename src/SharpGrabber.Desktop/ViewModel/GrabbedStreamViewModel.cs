using DotNetTools.SharpGrabber.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SharpGrabber.Desktop.ViewModel
{
    public class GrabbedStreamViewModel : BaseViewModel
    {
        public GrabbedStreamViewModel(GrabbedStreamMetadata stream)
        {
            Stream = stream;
            Name = stream.Name;
            Consideration = stream.Resolution?.ToString();
        }

        public GrabbedStreamMetadata Stream { get; }

        public string Consideration { get; } = string.Empty;
    }
}
