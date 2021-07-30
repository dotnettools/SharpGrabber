using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotNetTools.SharpGrabber.Grabbed;
using SharpGrabber.Desktop.UI;
using SharpGrabber.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpGrabber.Desktop.Components
{
    public class MediaResourceView : UserControl
    {
        #region Fields
        private Button btnDownload, btnCopyLink;
        private DrawingPresenter iconCheck, iconVideo, iconAudio, iconCreate;
        #endregion

        #region Properties
        public GrabbedMediaViewModel GrabbedMedia => DataContext as GrabbedMediaViewModel;

        public MainWindow MainWindow => Program.MainWindow;
        #endregion

        public MediaResourceView() { }

        public MediaResourceView(GrabbedMediaViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            var channels = GrabbedMedia.Media.Channels;
            DrawingPresenter icon;

            if (viewModel.IsComposition)
            {
                icon = iconCreate;
                btnCopyLink.IsVisible = false;
                ((TextBlock)btnDownload.Content).Text = "Download & convert";
            }
            else
                switch (channels)
                {
                    case MediaChannels.Both:
                        icon = iconCheck;
                        break;

                    case MediaChannels.Audio:
                        icon = iconAudio;
                        break;

                    case MediaChannels.Video:
                        icon = iconVideo;
                        break;

                    default:
                        throw new NotSupportedException($"Media channel of {channels} is not supported.");
                }
            icon.IsVisible = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            btnDownload = this.Find<Button>("btnDownload");
            btnCopyLink = this.Find<Button>("btnCopyLink");
            iconCheck = this.Find<DrawingPresenter>("iconCheck");
            iconVideo = this.Find<DrawingPresenter>("iconVideo");
            iconAudio = this.Find<DrawingPresenter>("iconAudio");
            iconCreate = this.Find<DrawingPresenter>("iconCreate");

            btnCopyLink.Subscribe(Button.ClickEvent, BtnCopyLink_Click);
            btnDownload.Subscribe(Button.ClickEvent, BtnDownload_Click);
        }

        private async void BtnCopyLink_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                await Application.Current.Clipboard.SetTextAsync(GrabbedMedia.Media.ResourceUri.ToString());
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
                Title = $"Download {GrabbedMedia.Name}",
                DefaultExtension = GrabbedMedia.Media.Format.Extension,
            };
            dlg.Filters.Add(new FileDialogFilter { Name = $"{GrabbedMedia.Media.Format.Extension.ToUpper()} Files", Extensions = new List<string> { GrabbedMedia.Media.Format.Extension } });

            var pureName = MainWindow.CurrentGrab.Title.Remove(Path.GetInvalidFileNameChars());

            if (GrabbedMedia.IsComposition)
                dlg.InitialFileName = $"{pureName}-{GrabbedMedia.Media.Resolution}-{GrabbedMedia.AttachTo.Container}.{GrabbedMedia.Media.Format.Extension}";
            else
                dlg.InitialFileName = $"{pureName}-{GrabbedMedia.Media.FormatTitle}.{GrabbedMedia.Media.Format.Extension}";
            var path = await dlg.ShowAsync(MainWindow);

            if (string.IsNullOrEmpty(path))
                return;

            IsEnabled = false;
            try
            {
                await MainWindow.Download(GrabbedMedia, path);
            }
            finally
            {
                IsEnabled = true;
            }
        }
    }
}
