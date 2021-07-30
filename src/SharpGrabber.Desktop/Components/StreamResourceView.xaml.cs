using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SharpGrabber.Desktop.UI;
using SharpGrabber.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpGrabber.Desktop.Components
{
    public class StreamResourceView : UserControl
    {
        #region Fields
        private Button btnDownload, btnCopyLink;
        #endregion

        #region Properties
        public GrabbedStreamViewModel GrabbedStream => DataContext as GrabbedStreamViewModel;

        public MainWindow MainWindow => Program.MainWindow;
        #endregion

        public StreamResourceView() { }

        public StreamResourceView(GrabbedStreamViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            btnDownload = this.Find<Button>("btnDownload");
            btnCopyLink = this.Find<Button>("btnCopyLink");

            btnCopyLink.Subscribe(Button.ClickEvent, BtnCopyLink_Click);
            btnDownload.Subscribe(Button.ClickEvent, BtnDownload_Click);
        }

        private async void BtnCopyLink_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                await Application.Current.Clipboard.SetTextAsync(GrabbedStream.Stream.ResourceUri.ToString());
            }
            catch (Exception exception)
            {
                MainWindow.ShowMessage("Copy link", exception.Message);
            }
        }

        private async void BtnDownload_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = $"Download {GrabbedStream.Name}",
                DefaultExtension = GrabbedStream.Stream.StreamFormat.Extension,
            };
            dlg.Filters.Add(new FileDialogFilter { Name = $"{GrabbedStream.Stream.OutputFormat.Extension.ToUpper()} Files", Extensions = new List<string> { GrabbedStream.Stream.OutputFormat.Extension } });

            var pureName = MainWindow.CurrentGrab.Title.Remove(Path.GetInvalidFileNameChars());

            dlg.InitialFileName = $"{pureName}.{GrabbedStream.Stream.OutputFormat.Extension}";
            var path = await dlg.ShowAsync(MainWindow);

            if (string.IsNullOrEmpty(path))
                return;

            IsEnabled = false;
            try
            {
                await MainWindow.Download(GrabbedStream, path);
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}
