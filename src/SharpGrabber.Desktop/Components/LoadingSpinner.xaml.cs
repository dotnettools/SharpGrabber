using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SharpGrabber.Desktop.Components
{
    public class LoadingSpinner : UserControl
    {
        public LoadingSpinner()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
