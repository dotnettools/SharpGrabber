using System;

namespace DotNetTools.SharpGrabber
{
    public class MimeInfo : IComparable
    {
        public string Name, Mime;

        public string[] Types;

        public int CompareTo(object obj)
        {
            return ((IComparable)Mime).CompareTo((obj as MimeInfo)?.Mime);
        }
    }
}
