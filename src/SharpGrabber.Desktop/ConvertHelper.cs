using System;
using System.Collections.Generic;
using System.Linq;
using DotNetTools.SharpGrabber.Converter;
using SharpGrabber.Desktop.ViewModel;

namespace SharpGrabber.Desktop
{
    public static class ConvertHelper
    {
        private static readonly Dictionary<string, HashSet<string>> ContainerMimeSupport = new Dictionary<string, HashSet<string>>(StringComparer.InvariantCultureIgnoreCase) {
            { "mp4",  new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "video/mp4", "audio/mp4" } },
            { "webm", new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "video/webm", "audio/webm" } }
        };

        public static GrabbedMediaViewModel[] SuggestConversions(IEnumerable<GrabbedMedia> grabs)
        {
            if (!IOHelper.FFMpegLoaded)
                return new GrabbedMediaViewModel[0];

            // To generate all possible conversions, all we need is to find a matching audio codec
            // for each and every video-only stream.
            var result = new List<GrabbedMediaViewModel>();

            // get all videos
            var videos = grabs.Where(g => g.Channels == MediaChannels.Video).ToArray();

            // iterate through videos
            foreach (var video in videos)
            {
                // find the best container for this video
                var container = video.Container;

                // get supported mime types
                if (!ContainerMimeSupport.ContainsKey(container))
                    continue;
                var supportedMimes = ContainerMimeSupport[container];

                // find the matching audio
                var audio = grabs.Where(g => g.Channels == MediaChannels.Audio && supportedMimes.Contains(g.Format.Mime)).FirstOrDefault();
                if (audio == null)
                    continue;

                // now we have both audio and video streams
                result.Add(new GrabbedMediaViewModel(video, audio));
            }

            return result.ToArray();
        }

        public static void Convert(GrabbedMedia videoStream, GrabbedMedia audioStream, string videoPath, string audioPath, string outputPath)
        {
            var builder = new MediaMerger(outputPath);
            builder.AddStreamSource(videoPath, MediaStreamType.Video);
            builder.AddStreamSource(audioPath, MediaStreamType.Audio);
            builder.OutputMimeType = videoStream.Format.Mime;
            builder.OutputShortName = videoStream.Format.Extension;
            builder.Build();
        }
    }
}
