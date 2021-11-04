using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpGrabber.BlackWidow.Tests.Assets
{
    internal static class AssetsAccessor
    {
        public static Stream GetStream(string fileName)
        {
            var type = typeof(AssetsAccessor);
            var resourceName = string.Join('.', new[] {
                type.Namespace,
                fileName.Replace('/', '.')
            });
            return type.Assembly.GetManifestResourceStream(resourceName);
        }

        public static string GetText(string fileName)
        {
            using var stream = GetStream(fileName);
            if (stream == null)
                return null;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
