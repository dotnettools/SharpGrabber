using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotNetTools.SharpGrabber.Hls
{
    public class PlaylistTagValue
    {
        public PlaylistTagValue(string key, string wholeValue, IDictionary<string, string> values)
        {
            Key = key;
            WholeValue = wholeValue;
            Values = values;
        }

        public string Key { get; }
        public string WholeValue { get; }
        public IDictionary<string, string> Values { get; }

        public static PlaylistTagValue Parse(string content)
        {
            var split = content.Split(new[] { ':' }, 2);
            var key = split[0];
            string val = split.Length > 1 ? split[1] : null;
            return new PlaylistTagValue(key, val, ParseValues(val));
        }

        private static readonly Regex _valueRegex = new Regex(@"(^|,)([\w\-]+)=(""([^""]+)""|([^,]+))", RegexOptions.Compiled);

        private static IDictionary<string, string> ParseValues(string content)
        {
            if (content == null)
                return null;
            var matches = _valueRegex.Matches(content);
            if (matches.Count == 0)
                return null;

            var dic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (Match match in matches)
            {
                var name = match.Groups[2].Value;
                var val = match.Groups[4].Value;
                if (string.IsNullOrEmpty(val))
                    val = match.Groups[5].Value;
                dic.Add(name, val);
            }
            return dic;
        }
    }
}
