using DotNetTools.SharpGrabber;
using SharpGrabber.Desktop.ViewModel;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpGrabber.Desktop
{

    public class MediaDownloader
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly GrabResult _grabResult;
        private readonly GrabbedMediaViewModel _viewModel;
        private readonly string _targetPath;

        public MediaDownloader(GrabbedMediaViewModel grabbedViewModel, string targetPath, GrabResult grabResult)
        {
            _grabResult = grabResult;
            _viewModel = grabbedViewModel;
            _targetPath = targetPath;
        }

        #region Internal Methods
        private async Task SingleDownload(Uri uri, Stream outputStream, string downloadingText = "Downloading {0}...")
        {
            using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength;
            if (totalBytes == 0)
                throw new Exception("No data to download.");
            else
                _viewModel.DownloadStatus = string.Format(downloadingText, UIHelpers.BytesToString(totalBytes.Value));
            var remainingBytes = totalBytes;
            var lastProgress = double.MinValue;

            using var originalStream = await response.Content.ReadAsStreamAsync();
            using var stream = await _grabResult.WrapStreamAsync(originalStream);
            var buffer = new byte[4096];
            while (true)
            {
                var countToRead = (int)Math.Min(remainingBytes ?? int.MaxValue, buffer.Length);
                var read = await stream.ReadAsync(buffer, 0, countToRead);
                if (read <= 0)
                    break;
                await outputStream.WriteAsync(buffer, 0, read);

                if (totalBytes.HasValue)
                {
                    remainingBytes -= read;
                    var progress = (totalBytes.Value - remainingBytes.Value) / (double)totalBytes.Value;
                    if (Math.Abs(progress - lastProgress) > 0.005)
                    {
                        _viewModel.DownloadProgress = progress;
                        lastProgress = progress;
                    }
                }
            }
            _viewModel.DownloadProgress = 1;
        }

        private async Task SingleDownload(Uri uri, string outputPath, string downloadingText = "Downloading {0}...")
        {
            try
            {
                using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await SingleDownload(uri, outputStream, downloadingText);
            }
            catch (Exception)
            {
                File.Delete(outputPath);
                throw;
            }
        }

        private async Task CompositeDownload()
        {
            var audioPath = IOHelper.GenerateTempPath();
            var videoPath = IOHelper.GenerateTempPath();
            var streamPaths = new[] { audioPath, videoPath };
            var videoStream = _viewModel.Media;
            var audioStream = _viewModel.AttachTo;

            try
            {
                _viewModel.DownloadStatus = "Preparing download...";
                await SingleDownload(videoStream.ResourceUri, videoPath, "Downloading video stream {0}...");
                await SingleDownload(audioStream.ResourceUri, audioPath, "Downloading audio stream {0}...");

                _viewModel.DownloadProgress = 0;
                _viewModel.DownloadStatus = "Composing output media...";

                await Task.Run(() =>
                {
                    ConvertHelper.Convert(videoStream, audioStream, videoPath, audioPath, _targetPath);
                });
            }
            finally
            {
                foreach (var path in streamPaths)
                    if (File.Exists(path))
                        File.Delete(path);
            }
        }
        #endregion

        #region Methods
        public async Task DownloadAsync()
        {
            // init
            var media = _viewModel.Media;
            _viewModel.IsDownloading = true;
            _viewModel.DownloadStatus = "Initializing...";
            _viewModel.DownloadPercent = 0;

            try
            {
                // is composite download?
                if (_viewModel.IsComposition)
                {
                    // Composed download
                    await CompositeDownload();
                    return;
                }

                // single download
                _viewModel.DownloadStatus = "Downloading...";
                await SingleDownload(media.ResourceUri, _targetPath);
            }
            finally
            {
                _viewModel.IsDownloading = false;
            }
        }
        #endregion
    }
}
