using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber.Internal
{
    internal class MimeInfo : IComparable
    {
        public string Name, Mime;

        public string[] Types;

        public int CompareTo(object obj)
        {
            return ((IComparable)Mime).CompareTo((obj as MimeInfo)?.Mime);
        }
    }
}
