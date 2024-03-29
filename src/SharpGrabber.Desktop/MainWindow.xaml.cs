﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Auth;
using DotNetTools.SharpGrabber.BlackWidow;
using DotNetTools.SharpGrabber.BlackWidow.Host;
using DotNetTools.SharpGrabber.BlackWidow.Interpreter.JavaScript;
using DotNetTools.SharpGrabber.BlackWidow.Repository;
using DotNetTools.SharpGrabber.Grabbed;
using DotNetTools.SharpGrabber.Odysee;
using FFmpeg.AutoGen;
using SharpGrabber.Desktop.Auth;
using SharpGrabber.Desktop.Components;
using SharpGrabber.Desktop.Utils;
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
        private readonly IBlackWidowGrabber _blackWidowGrabber;
        private TaskCompletionSource<GrabberBasicCredentials> _activeBasicAuth;
        private TaskCompletionSource<string> _active2fa;
        private bool _uiEnabled = true;
        private TextBox tbUrl;
        private TextBlock tbPlaceholder, txtGrab, tbGrabbers;
        private Button btnGrab, btnPaste, btnSaveImages;
        private LoadingSpinner spGrab;
        private Grid overlayRoot, noContent;
        private Border overlayContent;
        private TextBlock txtMsgTitle, txtMsgContent, txtTitle, txtBasicAuthTitle, txt2faTitle, txt2faText;
        private MenuItem miAbout, miLoadScript;
        private Button btnMsgOk, btnAuthOk, btn2faOk;
        private TextBlock txtMediaTitle;
        private TextBlock[] txtCol = new TextBlock[3];
        private TextBox txtAuthUsername, txtAuthPassword, txt2fa;
        private Image img;
        private LoadingSpinner imgSpinner;
        private StackPanel resourceContainer;
        private Border basicInfo;
        private Grid messageBox, basicAuthDlg, tfaDlg;
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
               .UseServices(b =>
               {
                   var auth = GrabberAuthenticationServiceBuilder.New()
                        .UseFileStore("auth.store")
                        .Build()
                        .RegisterInstagramHandler(new InstagramAuthenticationHandlerInterface(this));
                   b.UseAuthenticationService(auth);
               })
               .Add(_blackWidowGrabber = Program.BlackWidow.Grabber)
               .AddYouTube()
               .AddHls()
               .AddXnxx()
               .AddXVideos()
               .AddOdysee()
               // .AddInstagram() <= Failing authentication
               .Build();

            Program.ScriptHost.OnAlert += ScriptHost_OnAlert;
            Program.ScriptHost.OnLog += ScriptHost_OnLog;

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
            txtBasicAuthTitle = this.FindControl<TextBlock>("txtBasicAuthTitle");
            txt2faTitle = this.FindControl<TextBlock>("txt2faTitle");
            txt2faText = this.FindControl<TextBlock>("txt2faText");
            miAbout = this.FindControl<MenuItem>("miAbout");
            miLoadScript = this.FindControl<MenuItem>("miLoadScript");
            img = this.FindControl<Image>("img");
            imgSpinner = this.FindControl<LoadingSpinner>("imgSpinner");
            basicInfo = this.FindControl<Border>("basicInfo");
            messageBox = overlayContent.FindControl<Grid>("messageBox");
            basicAuthDlg = overlayContent.FindControl<Grid>("basicAuthDlg");
            tfaDlg = overlayContent.FindControl<Grid>("tfaDlg");
            resourceContainer = this.FindControl<StackPanel>("resourceContainer");
            txtAuthUsername = overlayRoot.FindControl<TextBox>("txtAuthUsername");
            txtAuthPassword = overlayRoot.FindControl<TextBox>("txtAuthPassword");
            txt2fa = overlayRoot.FindControl<TextBox>("txt2fa");
            btnAuthOk = overlayRoot.FindControl<Button>("btnAuthOk");
            btn2faOk = overlayRoot.FindControl<Button>("btn2faOk");

            this.Subscribe(KeyDownEvent, MainWindow_KeyDown);
            tbUrl.Subscribe(GotFocusEvent, TbUrl_GotFocus);
            tbUrl.Subscribe(LostFocusEvent, TbUrl_LostFocus);
            tbGrabbers.Subscribe(PointerReleasedEvent, TbGrabbers_Click);
            btnGrab.Subscribe(Button.ClickEvent, BtnGrab_Click);
            btnPaste.Subscribe(Button.ClickEvent, BtnPaste_Click);
            btnSaveImages.Subscribe(Button.ClickEvent, BtnSaveImages_Click);
            btnMsgOk.Subscribe(Button.ClickEvent, BtnOk_Click);
            btnAuthOk.Subscribe(Button.ClickEvent, BtnAuthOk_Click);
            btn2faOk.Subscribe(Button.ClickEvent, Btn2faOk_Click);
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
                .AppendLine("Copyright © 2023 SharpGrabber contributors");
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
                Console.WriteLine(exception.ToString());
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
                Console.WriteLine(exception.ToString());
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
            SetDialogVisibility(true);
            txtMsgTitle.Text = title;
            txtMsgContent.Text = text;
            messageBox.IsVisible = true;
            btnMsgOk.Focus();
        }

        public Task<GrabberBasicCredentials> ShowBasicAuthDialog(string title = "Basic Authentication")
        {
            SetDialogVisibility(true);
            if (_activeBasicAuth != null)
                throw new InvalidOperationException("Another basic auth dialog is currently active.");
            txtBasicAuthTitle.Text = title;
            basicAuthDlg.IsVisible = true;
            txtAuthUsername.Text = string.Empty;
            txtAuthPassword.Text = string.Empty;
            txtAuthUsername.Focus();
            _activeBasicAuth = new();
            return _activeBasicAuth.Task;
        }

        public Task<string> ShowTwoFactorAuthDialog(string title = "Two Factor Authentication", string text = "Two Factor Code")
        {
            SetDialogVisibility(true);
            if (_active2fa != null)
                throw new InvalidOperationException("Another two factor auth dialog is currently active.");
            txt2faTitle.Text = title;
            txt2faText.Text = text;
            tfaDlg.IsVisible = true;
            txt2fa.Text = string.Empty;
            txt2fa.Focus();
            _active2fa = new();
            return _active2fa.Task;
        }

        public void CloseDialog()
        {
            SetDialogVisibility(false);
            messageBox.IsVisible = false;
            basicAuthDlg.IsVisible = false;
            tfaDlg.IsVisible = false;
            _activeBasicAuth?.SetResult(null);
            _activeBasicAuth = null;
            _active2fa?.SetResult(null);
            _active2fa = null;
        }

        private void SetDialogVisibility(bool visible = true)
        {
            overlayRoot.IsVisible = visible;
            overlayContent.Opacity = visible ? 1 : 0;
        }
        #endregion

        private void TbGrabbers_Click(object sender, RoutedEventArgs e)
        {
            static string GrabberToString(IGrabber grabber)
            {
                return $"- {grabber.Name} {(string.IsNullOrEmpty(grabber.StringId) ? null : $" ({grabber.StringId})")}";
            }

            var sb = new StringBuilder()
                .AppendLine("Native Grabbers:");
            foreach (var g in _grabber.GetRegisteredGrabbers())
                sb.AppendLine(GrabberToString(g));

            sb.AppendLine()
                .AppendLine("BlackWidow Grabbers (locally available):");
            foreach (var g in _blackWidowGrabber.GetScriptGrabbers())
                sb.AppendLine(GrabberToString(g));

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
                var innerMostException = exception.FindInnerMostException();
                ShowMessage("Grab error", innerMostException.Message);
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

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void BtnAuthOk_Click(object sender, RoutedEventArgs e)
        {
            _activeBasicAuth?.SetResult(new GrabberBasicCredentials(txtAuthUsername.Text, txtAuthPassword.Text));
            _activeBasicAuth = null;
            CloseDialog();
        }

        private void Btn2faOk_Click(object sender, RoutedEventArgs e)
        {
            _active2fa?.SetResult(txt2fa.Text);
            _active2fa = null;
            CloseDialog();
        }

        private void MainWindow_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                DisplayAbout();
            if (e.Key == Key.Escape)
                CloseDialog();
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
            DisplayAbout();
            // txtTitle.ContextMenu.Open();
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
            // not implemented
        }

        private void MiAbout_Click(object sender, RoutedEventArgs e)
        {
            DisplayAbout();
        }

        private void MainWindow_Initialized(object sender, System.EventArgs e)
        {
            Title = Constants.AppFullName;
        }

        private void ScriptHost_OnLog(ConsoleLog logObject)
        {
        }

        private void ScriptHost_OnAlert(object input)
            => Dispatcher.UIThread.InvokeAsync(() =>
            {
                ShowMessage("Script", input?.ToString());
            });
    }
}