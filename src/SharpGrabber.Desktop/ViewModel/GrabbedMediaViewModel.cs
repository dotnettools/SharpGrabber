using DotNetTools.SharpGrabber.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SharpGrabber.Desktop.ViewModel
{
    public class GrabbedMediaViewModel : INotifyPropertyChanged
    {
        #region Fields
        private readonly Dictionary<object, object> _properties = new Dictionary<object, object>();

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public GrabbedMedia Media { get; }

        /// <summary>
        /// The stream to attach <see cref="Media"/> to - Only applicable to composite types.
        /// </summary>
        public GrabbedMedia AttachTo { get; }

        public string Name { get; }

        public string Consideration { get; } = string.Empty;

        public bool IsDownloading
        {
            get => Get<bool>(nameof(IsDownloading));
            set => Set(nameof(IsDownloading), value);
        }

        public double DownloadProgress
        {
            get => Get<double>("DownloadProgress", 0);
            set
            {
                Set(nameof(DownloadProgress), value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadPercent)));
            }
        }

        public double DownloadPercent
        {
            get => DownloadProgress * 100;
            set => DownloadProgress = value / 100;
        }

        public string DownloadStatus
        {
            get => Get<string>(nameof(DownloadStatus));
            set => Set(nameof(DownloadStatus), value);
        }

        /// <summary>
        /// Returns whether or not this view model is of composited type.
        /// </summary>
        public bool IsComposition => AttachTo != null;
        #endregion

        #region Constructor
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
        #endregion

        #region Internal Methods
        private T Get<T>(string propertyName, T @default = default)
        {
            if (_properties.ContainsKey(propertyName))
                return (T)_properties[propertyName];
            return @default;
        }

        private void Set(string propertyName, object value)
        {
            if (_properties.ContainsKey(propertyName))
                _properties[propertyName] = value;
            else
                _properties.Add(propertyName, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
