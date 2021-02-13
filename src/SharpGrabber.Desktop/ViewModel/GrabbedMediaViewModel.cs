using DotNetTools.SharpGrabber.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SharpGrabber.Desktop.ViewModel
{
    public class GrabbedMediaViewModel : BaseViewModel
    {
        public GrabbedMediaViewModel(GrabbedMedia media, GrabbedMedia attachTo = null)
        {
            Media = media;
            AttachTo = attachTo;
            Name = $"{media.Container?.ToUpperInvariant()} {media.Resolution}".Trim();

            if (IsComposition)
                Consideration = "Full media after conversion";
            else if (media.Channels == MediaChannels.Both)
                Consideration = "Full Media";
            else
                Consideration = (media.Channels.HasAudio() ? "Audio" : "Video") + " Only";
        }

        public GrabbedMedia Media { get; }

        /// <summary>
        /// The stream to attach <see cref="Media"/> to - Only applicable to composite types.
        /// </summary>
        public GrabbedMedia AttachTo { get; }

        public string Consideration { get; } = string.Empty;

        /// <summary>
        /// Returns whether or not this view model is of composited type.
        /// </summary>
        public bool IsComposition => AttachTo != null;
    }
}
