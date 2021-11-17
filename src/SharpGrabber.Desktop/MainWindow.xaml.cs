using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.BlackWidow.Builder;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using DotNetTools.SharpGrabber.Grabbed;
using FFmpeg.AutoGen;
using SharpGrabber.Desktop.Components;
using SharpGrabber.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpGrabber.Desktop
{
    public class MainWindow : Window
    {
        private static readonly HttpClient _client = new HttpClient();

        #region Fields
        private readonly IMultiGrabber _grabber;
        private bool _uiEnabled = true;
        private TextBox tbUrl;
        private TextBlock tbPlaceholder, txtGrab, tbGrabbers;
        private Button btnGrab, btnPaste, btnSaveImages;
        private LoadingSpinner spGrab;
        private Grid overlayRoot, noContent;
        private Border overlayContent;
        private TextBlock txtMsgTitle, txtMsgContent, txtTitle;
        private MenuItem miAbout, miLoadScript;
        private Button btnMsgOk;
        private TextBlock txtMediaTitle;
        private TextBlock[] txtCol = new TextBlock[3];
        private Image img;
        private LoadingSpinner imgSpinner;
        private StackPanel resourceContainer;
        private Border basicInfo;
        #endregion

        #region Properties

        /// <summary>
        /// Result of the last grab
        /// </summary>
        public GrabResult CurrentGrab { get; set; }

        public bool IsUIEnabled
        {
            get => _uiEnabled;
            set
            {
                if (_uiEnabled == value)
                    return;
                _uiEnabled = value;
                btnGrab.IsEnabled = btnPaste.IsEnabled = btnSaveImages.IsEnabled = value;
                txtGrab.IsVisible = value;
                spGrab.IsVisible = !value;
                resourceContainer.IsEnabled = value;
            }
        }

        #endregion

        public MainWindow()
        {
            Initialized += MainWindow_Initialized;

            _grabber = GrabberBuilder.New()
               .UseDefaultServices()
               .Add(Program.BlackWidow.Grabber)
               .AddYouTube()
               .AddInstagram()
               .AddHls()
               .AddXnxx()
               .AddXVideos()
               .Build();

            InitializeComponent();
            basicInfo.IsVisible = resourceContainer.IsVisible = false;
            CheckLibrary();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            tbUrl = this.FindControl<TextBox>("tbUrl");
            tbPlaceholder = this.FindControl<TextBlock>("tbPlaceholder");
            btnGrab = this.FindControl<Button>("btnGrab");
            btnPaste = this.FindControl<Button>("btnPaste");
            btnSaveImages = this.FindControl<Button>("btnSaveImages");
            tbGrabbers = this.FindControl<TextBlock>("tbGrabbers");
            spGrab = this.FindControl<LoadingSpinner>("spGrab");
            txtGrab = this.FindControl<TextBlock>("txtGrab");
            overlayRoot = this.FindControl<Grid>("overlayRoot");
            overlayContent = this.FindControl<Border>("overlayContent");
            noContent = this.FindControl<Grid>("noContent");
            txtMsgTitle = this.FindControl<TextBlock>("txtMsgTitle");
            txtMsgContent = this.FindControl<TextBlock>("txtMsgContent");
            btnMsgOk = this.FindControl<Button>("btnMsgOk");
            for (var i = 0; i < 3; i++)
                txtCol[i] = this.FindControl<TextBlock>($"txtCol{i}");
            txtMediaTitle = this.FindControl<TextBlock>("txtMediaTitle");
            txtTitle = this.FindControl<TextBlock>("txtTitle");
            miAbout = this.FindControl<MenuItem>("miAbout");
            miLoadScript = this.FindControl<MenuItem>("miLoadScript");
            img = this.FindControl<Image>("img");
            imgSpinner = this.FindControl<LoadingSpinner>("imgSpinner");
            basicInfo = this.FindControl<Border>("basicInfo");
            resourceContainer = this.FindControl<StackPanel>("resourceContainer");

            this.Subscribe(KeyDownEvent, MainWindow_KeyDown);
            tbUrl.Subscribe(GotFocusEvent, TbUrl_GotFocus);
            tbUrl.Subscribe(LostFocusEvent, TbUrl_LostFocus);
            tbGrabbers.Subscribe(PointerReleasedEvent, TbGrabbers_Click);
            btnGrab.Subscribe(Button.ClickEvent, BtnGrab_Click);
            btnPaste.Subscribe(Button.ClickEvent, BtnPaste_Click);
            btnSaveImages.Subscribe(Button.ClickEvent, BtnSaveImages_Click);
            btnMsgOk.Subscribe(Button.ClickEvent, BtnOk_Click);
            txtTitle.Subscribe(PointerPressedEvent, TxtTitle_Click);
            miAbout.Subscribe(MenuItem.ClickEvent, MiAbout_Click);
            miLoadScript.Subscribe(MenuItem.ClickEvent, MiLoadScript_Click);
        }

        #region Internal Methods

        private void CheckLibrary()
        {
            try
            {
                if (!IOHelper.TryLoadFFMpeg())
                    throw new Exception(
                        "Could not locate path to the ffmpeg library. The following paths were looked up:" +
                        Environment.NewLine
                        + "- " + string.Join($"{Environment.NewLine}- ",
                            IOHelper.SuggestedFFMpegDirectories.ToArray()));
            }
            catch (Exception exception)
            {
                ShowMessage("FFMpeg error",
                    $"{exception.Message}{Environment.NewLine}{Environment.NewLine}As a result, conversion capability is disabled. Please check if ffmpeg shared libraries with the matching architecture are available at one of the directories above and restart the application.");
            }
        }

        private void DisplayAbout()
        {
            var sb = new StringBuilder();
            sb
                .Append(Constants.AppName).AppendFormat(" [{0}-bit process]", Environment.Is64BitProcess ? 64 : 32)
                .AppendLine()
                .Append("Version ").AppendLine(Constants.AppVersion.ToString())
                .Append("FFMpeg Version: ").Append(IOHelper.FFMpegLoaded ? ffmpeg.av_version_info() : "<Not Loaded>")
                .AppendLine()
                .AppendLine()
                .AppendLine("Copyright © 2021 Javid Shoaei (javidsh.ir) and other contributors");
            ShowMessage("About", sb.ToString());
        }

        public async Task Download(GrabbedMediaViewModel grabbed, string path)
        {
            try
            {
                var downloader = new MediaDownloader(grabbed, path, CurrentGrab);
                await downloader.DownloadAsync();
            }
            catch (Exception exception)
            {
                ShowMessage("Download error", exception.Message);
            }
        }

        public async Task Download(GrabbedStreamViewModel grabbed, string path)
        {
            try
            {
                var downloader = new StreamDownloader(grabbed, path, CurrentGrab);
                await downloader.DownloadAsync();
            }
            catch (Exception exception)
            {
                ShowMessage("Download error", exception.Message);
            }
        }

        private void DisplayMetadataInfo(List<KeyValuePair<string, string>> pairs)
        {
            const int ELEMENT_CAPACITY = 3;
            var sb = new StringBuilder();
            var index = 0;
            var count = 0;

            foreach (var txt in txtCol)
                txt.IsVisible = false;

            foreach (var pair in pairs)
            {
                if (string.IsNullOrEmpty(pair.Value))
                    continue;
                if (index >= txtCol.Length)
                    return;

                sb.AppendLine($"{pair.Key}: {pair.Value}");
                if (++count >= ELEMENT_CAPACITY)
                {
                    txtCol[index].Text = sb.ToString();
                    txtCol[index].IsVisible = true;
                    sb.Clear();
                    index++;
                    count = 0;
                }
            }

            if (count > 0 && index < txtCol.Length)
            {
                txtCol[index].Text = sb.ToString();
                txtCol[index].IsVisible = true;
            }
        }

        private void LoadMedia(IEnumerable<IGrabbed> allGrabbedResources)
        {
            // init
            var medias = allGrabbedResources.OfType<GrabbedMedia>().ToList();
            resourceContainer.Children.Clear();

            // add sugguested conversions
            foreach (var vm in ConvertHelper.SuggestConversions(medias))
            {
                var view = new MediaResourceView(vm);
                resourceContainer.Children.Add(view);
            }

            // add views for grabbed objects
            foreach (var grab in medias)
            {
                var view = new MediaResourceView(new GrabbedMediaViewModel(grab));
                resourceContainer.Children.Add(view);
            }

            // present streams
            var streams = allGrabbedResources.OfType<GrabbedHlsStreamMetadata>().ToList();
            foreach (var streamMetadata in streams)
            {
                var view = new StreamResourceView(new GrabbedStreamViewModel(streamMetadata));
                resourceContainer.Children.Add(view);
            }

            // present stream references
            var streamRefs = allGrabbedResources.OfType<GrabbedHlsStreamReference>().ToList();
            foreach (var streamRef in streamRefs)
            {
                var view = new StreamReferenceView(new GrabbedStreamRefViewModel(streamRef));
                resourceContainer.Children.Add(view);
            }
        }

        private async Task LoadGrabResult(GrabResult result)
        {
            // init
            CurrentGrab = result;
            var images = result.Resources<GrabbedImage>().ToArray();
            var mediaFiles = result.Resources<GrabbedMedia>().ToArray();
            noContent.IsVisible = false;

            // basic info
            var metadata = new List<KeyValuePair<string, string>>();
            txtMediaTitle.Text = result.Title;
            var info = result.Resource<GrabbedInfo>();
            metadata.Add(new KeyValuePair<string, string>("Length", info?.Length?.ToString()));
            metadata.Add(new KeyValuePair<string, string>("Author", info?.Author));
            metadata.Add(new KeyValuePair<string, string>("Views", info?.ViewCount?.ToString("N0")));
            metadata.Add(new KeyValuePair<string, string>("Creation Date", result.CreationDate?.ToShortDateString()));
            DisplayMetadataInfo(metadata);

            // load resources
            LoadMedia(result.Resources);

            // display media info for the first time
            resourceContainer.IsVisible = true;
            basicInfo.IsVisible = images.Any() || mediaFiles.Any();

            // load thumbnail
            img.Source = null;
            if (images.Length > 0)
            {
                var thumbnail = images.Where(i => i.Type == GrabbedImageType.Thumbnail).FirstOrDefault() ??
                                images.FirstOrDefault();
                imgSpinner.IsVisible = true;
                try
                {
                    using var response = await _client.GetAsync(thumbnail.ResourceUri);
                    if (response.IsSuccessStatusCode)
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var bitmap = new Bitmap(stream);
                            img.Source = bitmap;
                        }
                }
                finally
                {
                    imgSpinner.IsVisible = false;
                }
            }
        }

        private async Task Grab(string uriText)
        {
            // parse uri
            if (!Uri.TryCreate(uriText, UriKind.Absolute, out var uri))
                throw new Exception("Please enter a valid URL.");

            // try grab
            var result = await _grabber.GrabAsync(uri, CancellationToken.None, new GrabOptions(GrabOptionFlags.All));

            // apply grab result
            _ = LoadGrabResult(result);
        }

        private async Task SaveImages(string folderPath)
        {
            IsUIEnabled = false;
            try
            {
                var images = CurrentGrab?.Resources<GrabbedImage>().ToArray();
                if (images == null || images.Length == 0)
                    return;

                var count = 0;
                var client = new HttpClient();
                foreach (var image in images)
                {
                    // download image
                    using var response = await client.GetAsync(image.ResourceUri);
                    if (!response.IsSuccessStatusCode)
                        continue;
                    var ext = response.Content.Headers.ContentType?.MediaType?.Split('/')?.LastOrDefault();
                    var path = Path.Combine(folderPath, $"image-{++count}.{ext}");
                    using var imageStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    await response.Content.CopyToAsync(imageStream);
                }

                ShowMessage("Save images", $"{count} images were successfully downloaded.");
            }
            catch (Exception exception)
            {
                ShowMessage("Image save error", exception.Message);
            }
            finally
            {
                IsUIEnabled = true;
            }
        }

        public void ShowMessage(string title, string text)
        {
            // overlayContent.Opacity = 0;
            overlayRoot.IsVisible = true;
            overlayContent.Opacity = 1;
            txtMsgTitle.Text = title;
            txtMsgContent.Text = text;
            btnMsgOk.Focus();
        }

        public void CloseMessage()
        {
            overlayContent.Opacity = 0;
            overlayRoot.IsVisible = false;
        }

        #endregion

        private void TbGrabbers_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var g in _grabber.GetRegisteredGrabbers())
            {
                sb.AppendLine(g.Name + (string.IsNullOrEmpty(g.StringId) ? null : $" ({g.StringId})"));
            }

            ShowMessage("Registered Grabbers", sb.ToString());
        }

        public async Task SetUrlAndGrab(string url)
        {
            IsUIEnabled = false;
            try
            {
                await Grab(url);
            }
            catch (Exception exception)
            {
                ShowMessage("Grab error", exception.Message);
            }
            finally
            {
                IsUIEnabled = true;
            }
        }

        private async void BtnGrab_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
            => await SetUrlAndGrab(tbUrl.Text);

        private async void BtnPaste_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var text = await Application.Current.Clipboard.GetTextAsync();
            if (string.IsNullOrEmpty(text))
            {
                ShowMessage("Clipboard error", "There is no text in the clipboard.");
                return;
            }
            else if (text.Length > 1024)
            {
                ShowMessage("Clipboard error", "The text in the clipboard is too long.");
                return;
            }

            tbUrl.Text = text.Replace("\r", string.Empty).Replace("\n", string.Empty);
            tbPlaceholder.IsVisible = false;

            if (Uri.IsWellFormedUriString(tbUrl.Text, UriKind.Absolute))
                BtnGrab_Click(btnGrab, null);
        }

        private async void BtnSaveImages_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var images = CurrentGrab?.Resources.Where(r => r is GrabbedImage).ToArray();
            if (images == null || images.Length == 0)
            {
                ShowMessage("Save images", "No images could be grabbed from the source.");
                return;
            }

            var dlg = new OpenFolderDialog
            {
                Title = "Save images",
            };
            var folder = await dlg.ShowAsync(this);
            if (string.IsNullOrEmpty(folder))
                return;

            await SaveImages(folder);
        }

        private void BtnOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CloseMessage();
        }

        private void MainWindow_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                DisplayAbout();
            if (e.Key == Key.Escape)
                CloseMessage();
        }

        private void TbUrl_LostFocus(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            tbPlaceholder.IsVisible = string.IsNullOrEmpty(tbUrl.Text);
        }

        private void TbUrl_GotFocus(object sender, Avalonia.Input.GotFocusEventArgs e)
        {
            tbPlaceholder.IsVisible = false;
        }

        private void TxtTitle_Click(object sender, PointerPressedEventArgs e)
        {
            txtTitle.ContextMenu.Open();
        }

        private async void MiLoadScript_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Load BlackWidow Script",
                Filters = new List<FileDialogFilter> {
                    new FileDialogFilter { Name = "JavaScript File", Extensions = new List<string> { "js" } }
                },
                AllowMultiple = false,
            };
            var fileNames = await dlg.ShowAsync(this);
            if (fileNames == null || fileNames.Length == 0)
                return;
            ShowMessage("You Selected", fileNames.First());
        }

        private void MiAbout_Click(object sender, RoutedEventArgs e)
        {
            DisplayAbout();
        }

        private void MainWindow_Initialized(object sender, System.EventArgs e)
        {
            Title = Constants.AppFullName;
        }
    }
}