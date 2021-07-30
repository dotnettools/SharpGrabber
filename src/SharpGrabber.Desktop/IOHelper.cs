using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SharpGrabber.Desktop
{
    public static class IOHelper
    {
        private static readonly Random _random = new Random();

        public static string BasePath { get; }

        public static readonly HashSet<string> SuggestedFFMpegDirectories = new HashSet<string>();

        static IOHelper()
        {
            // get base path
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            basePath = Path.Combine(basePath, "SharpGrabber/temp");
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            BasePath = basePath;

            // add suggested ffmpeg paths
            var srcPath = Assembly.GetExecutingAssembly().Location;
            var srcDir = Path.GetDirectoryName(srcPath);
            SuggestedFFMpegDirectories.Add(srcDir);
            SuggestedFFMpegDirectories.Add(Path.Combine(srcDir, "ffmpeg"));
            SuggestedFFMpegDirectories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ffmpeg"));
            SuggestedFFMpegDirectories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ffmpeg"));
            SuggestedFFMpegDirectories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ffmpeg"));
        }

        public static string GetTempPath(string subPath) => Path.Combine(BasePath, subPath);

        public static string GenerateTempPath()
        {
            while (true)
            {
                var number = _random.Next();
                var fileName = $"temp-{number}";
                var path = GetTempPath(fileName);
                if (!File.Exists(path))
                    return path;
            }
        }

        public static bool FFMpegLoaded { get; private set; } = false;

        public static bool TryLoadFFMpeg()
        {
            foreach (var path in SuggestedFFMpegDirectories)
            {
                if (Directory.Exists(path))
                {
                    ffmpeg.RootPath = path;
                    try
                    {
                        ffmpeg.avutil_version();
                    }
                    catch (DllNotFoundException)
                    {
                        continue;
                    }
                    FFMpegLoaded = true;
                    break;
                }
            }

            return FFMpegLoaded;
        }
    }
}
