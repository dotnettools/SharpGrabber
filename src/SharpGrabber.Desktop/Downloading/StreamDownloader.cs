using Avalonia.Media;
using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Converter;
using DotNetTools.SharpGrabber.Media;
using Microsoft.VisualBasic.CompilerServices;
using SharpGrabber.Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpGrabber.Desktop
{
    public class StreamDownloader
    {
        private readonly GrabResult _grabResult;
        private readonly GrabbedStreamViewModel _viewModel;
        private readonly string _targetPath;

        public StreamDownloader(GrabbedStreamViewModel viewModel, string targetPath, GrabResult grabResult)
        {
            _grabResult = grabResult;
            _viewModel = viewModel;
            _targetPath = targetPath;
        }

        public async Task DownloadAsync()
        {
            // init
            var segmentFiles = new List<string>();
            var stream = await _viewModel.Stream.Stream.ResolveAsync();
            _viewModel.IsDownloading = true;
            _viewModel.DownloadStatus = "Initializing...";
            _viewModel.DownloadPercent = 0;

            try
            {
                // download all segments
                var segmentIndex = 0;
                var downloaded = new Reference<long>(0);
                var segmentCount = new Reference<int>(0);
                foreach (var segment in stream.Segments)
                {
                    _viewModel.DownloadStatus = $"Downloading segments ({segmentIndex}/{stream.Segments.Count})...";
                    var segmentPath = _targetPath + $"_{segmentIndex}.segment";
                    segmentFiles.Add(segmentPath);
                    segmentIndex++;

                    using var outStream = new FileStream(segmentPath, FileMode.Create, FileAccess.Write);
                    await DownloadAsync(segment, outStream, stream.Segments.Count, downloaded, segmentCount);
                }

                // create output file
                _viewModel.DownloadProgress = 0;
                _viewModel.DownloadStatus = "Building output file...";
                await CreateOutputFile(segmentFiles);
            }
            finally
            {
                foreach (var path in segmentFiles)
                    if (File.Exists(path))
                        File.Delete(path);
                _viewModel.IsDownloading = false;
            }
        }

        private Task CreateOutputFile(IEnumerable<string> segmentFiles)
        {
            var concatenator = new MediaConcatenator(_targetPath)
            {
                OutputMimeType = _viewModel.Stream.OutputFormat.Mime,
                OutputExtension = _viewModel.Stream.OutputFormat.Extension,
            };
            foreach (var segmentFile in segmentFiles)
                concatenator.AddSource(segmentFile);
            concatenator.Build();
            return Task.CompletedTask;
        }

        private async Task DownloadAsync(MediaSegment segment, Stream outStream, int totalSegmentCount,
            Reference<long> overallDownloaded, Reference<int> downloadedSegmentCount)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(segment.Uri, HttpCompletionOption.ResponseHeadersRead);
            using var originalStream = await response.Content.ReadAsStreamAsync();
            using var stream = await _grabResult.WrapStreamAsync(originalStream);
            var contentLength = response.Content.Headers.ContentLength;

            if (contentLength == null)
            {
                await stream.CopyToAsync(outStream);
                return;
            }

            var avgSegmentSize = downloadedSegmentCount > 0 ? overallDownloaded / (double)downloadedSegmentCount : contentLength.Value;
            var downloaded = 0L;
            var totalBytes = avgSegmentSize * totalSegmentCount;

            const int BUFFER_LENGTH = 4096;
            var buffer = new byte[BUFFER_LENGTH];
            while (true)
            {
                var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0)
                    break;
                await outStream.WriteAsync(buffer, 0, read);
                downloaded += read;
                _viewModel.DownloadProgress = (avgSegmentSize * downloadedSegmentCount + downloaded) / totalBytes;
            }

            overallDownloaded.Value += contentLength.Value;
            downloadedSegmentCount.Value++;
        }
    }
}
