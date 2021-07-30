namespace SharpGrabber.Desktop.UI
{
    public static class StringHelpers
    {
        public static string Remove(this string str, params char[] chars)
        {
            if (str == null)
                return null;
            foreach (var ch in chars)
                str = str.Replace(ch.ToString(), string.Empty);
            return str;
        }
    }
}
