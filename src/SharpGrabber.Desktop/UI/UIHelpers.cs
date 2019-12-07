using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpGrabber.Desktop
{
    public static class UIHelpers
    {
        public static void Subscribe(this Interactive targetControl, RoutedEvent @event, Delegate handler, bool handledToo = true)
        {
            targetControl.AddHandler(@event, handler, RoutingStrategies.Bubble | RoutingStrategies.Direct, handledToo);
        }

        public static void Subscribe<TEventArgs>(this Interactive targetControl, RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler, bool handledToo = true) where TEventArgs : RoutedEventArgs
        {
            targetControl.Subscribe(routedEvent, (Delegate)handler, handledToo);
        }

        private static readonly string[] SIZE_SUFFIXES = { "B", "KB", "MB", "GB", "TB" };

        public static string BytesToString(long byteCount)
        {
            if (byteCount == 0)
                return "0 " + SIZE_SUFFIXES[0];
            long bytes = Math.Abs(byteCount);
            int place = (int)Math.Floor(Math.Log(bytes, 1024));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            num *= Math.Sign(byteCount);
            return $"{num} {SIZE_SUFFIXES[place]}";
        }
    }
}
