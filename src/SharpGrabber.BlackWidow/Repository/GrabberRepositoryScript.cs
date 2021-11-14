using DotNetTools.SharpGrabber.BlackWidow.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository
{
    /// <summary>
    /// Default implementation for <see cref="IGrabberRepositoryScript"/>
    /// </summary>
    public class GrabberRepositoryScript : IGrabberRepositoryScript
    {
        private string[] _regularExpressionStrings;
        private Regex[] _regularExpressions;

        public string Id { get; set; }

        public string Name { get; set; }

        public GrabberScriptType Type { get; set; }

        public string Version { get; set; }

        public bool IsDeprecated { get; set; }

        public string[] SupportedRegularExpressions
        {
            get => _regularExpressionStrings;
            set
            {
                if (_regularExpressionStrings == value)
                    return;
                _regularExpressionStrings = value;
                _regularExpressions = value?.Select(s => new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase)).ToArray();
            }
        }

        public int ApiVersion { get; set; } = 1;

        public bool IsMatch(Uri uri)
        {
            // test for potential general support
            if (_regularExpressions == null || _regularExpressions.Length == 0)
                return true;

            var uriString = uri.ToString();
            return _regularExpressions.Any(regex => regex.IsMatch(uriString));
        }

        public override int GetHashCode()
        {
            return HashCodeUtils.Compute(Id);
        }

        public override bool Equals(object obj)
        {
            return EqualityUtils.Equals<GrabberRepositoryScript>(this, obj,
                o => o.Id,
                o => o.Name,
                o => o.Type,
                o => o.Version,
                o => o.IsDeprecated,
                o => o.SupportedRegularExpressions,
                o => o.ApiVersion);
        }
    }
}
