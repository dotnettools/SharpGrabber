using System.Collections.Generic;
using System.ComponentModel;

namespace SharpGrabber.Desktop.ViewModel
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<object, object> _properties = new Dictionary<object, object>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; protected set; }

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
                InvokePropertyChanged(new PropertyChangedEventArgs(nameof(DownloadPercent)));
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

        protected void InvokePropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        protected T Get<T>(string propertyName, T @default = default)
        {
            if (_properties.ContainsKey(propertyName))
                return (T)_properties[propertyName];
            return @default;
        }

        protected void Set(string propertyName, object value)
        {
            if (_properties.ContainsKey(propertyName))
                _properties[propertyName] = value;
            else
                _properties.Add(propertyName, value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
