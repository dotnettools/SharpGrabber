using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotNetTools.SharpGrabber.Media;
using SharpGrabber.Desktop;
using SharpGrabber.Desktop.UI;
using SharpGrabber.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpGrabber.Desktop.Components
{
    public class StreamReferenceView : UserControl
    {
        #region Fields
        private Button btnDownload, btnCopyLink;
        private DrawingPresenter iconCheck, iconVideo, iconAudio, iconCreate;
        #endregion

        #region Properties
        public GrabbedStreamRefViewModel Ref => DataContext as GrabbedStreamRefViewModel;

        public MainWindow MainWindow => Program.MainWindow;
        #endregion

        public StreamReferenceView() { }

        public StreamReferenceView(GrabbedStreamRefViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            iconCheck.IsVisible = true;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            btnDownload = this.Find<Button>("btnDownload");
            btnCopyLink = this.Find<Button>("btnCopyLink");
            iconCheck = this.Find<DrawingPresenter>("iconCheck");

            btnCopyLink.Subscribe(Button.ClickEvent, BtnCopyLink_Click);
            btnDownload.Subscribe(Button.ClickEvent, BtnDownload_Click);
        }

        private async void BtnCopyLink_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                await Application.Current.Clipboard.SetTextAsync(Ref.Reference.ResourceUri.ToString());
            }
            catch (Exception exception)
            {
                MainWindow.ShowMessage("Copy link", exception.Message);
            }
        }

        private async void BtnDownload_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await MainWindow.SetUrlAndGrab(Ref.Reference.ResourceUri.ToString());
        }
    }
}
