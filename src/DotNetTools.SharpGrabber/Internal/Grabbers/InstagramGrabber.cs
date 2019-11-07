using DotNetTools.SharpGrabber.Exceptions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Internal.Grabbers
{
    /// <summary>
    /// Represents an Instagram <see cref="IGrabber"/>.
    /// </summary>
    public class InstagramGrabber : BaseGrabber
    {
        #region Fields
        private readonly Regex _idPattern = new Regex(@"^https?://(www\.)?instagram\.com/[A-Za-z0-9]/([A-Za-z0-9]+)(/.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        #endregion

        #region Properties
        public override string Name { get; } = "Instagram";

        /// <summary>
        /// Represents the template of standard Instagram links. This value will be formatted using <see cref="String.Format"/>
        /// passing an Instagram post identifier containing alpha-numeric characters.
        /// </summary>
        public virtual string StandardUrlTemplate { get; set; } = "https://www.instagram.com/p/{0}/";
        #endregion

        #region Internal Methods
        protected virtual Uri MakeStandardInstagramUri(string id)
        {
            return new Uri(string.Format(StandardUrlTemplate, id));
        }

        protected virtual string GrabId(Uri uri)
        {
            var uriString = uri.ToString();
            var match = _idPattern.Match(uriString);
            if (!match.Success)
                return null;
            return match.Groups[2].Value;
        }

        /// <summary>
        /// Validates response of the page request. For example, in the beginning the returned status code is checked.
        /// </summary>
        protected virtual void CheckResponse(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                throw new GrabException($"A server error occurred while retrieving Instagram content. Server returned {response.StatusCode} {response.ReasonPhrase}.");
        }

        protected virtual IDictionary<string, string> ParsePage(Stream responseStream)
        {
            var dictionary = new Dictionary<string, string>();


            // parse page html
            var doc = new HtmlDocument();
            doc.Load(responseStream);

            // update result
            var nodes = doc.DocumentNode.SelectNodes("//meta[starts-with(@property, 'og:') and @property and @content]");
            if (nodes == null)
                throw new GrabException("Failed to obtain metadata from the Instagram page.");

            foreach (var node in nodes)
            {
                dictionary.Add(node.Attributes["property"].Value, node.Attributes["content"].Value);
            }

            return dictionary;
        }

        protected virtual IGrabbed[] GrabUsingMetadata(IDictionary<string, string> metaData)
        {
            var grabList = new List<IGrabbed>();

            return grabList.ToArray();
        }
        #endregion

        #region Methods
        public async override Task<IEnumerable<IGrabbed>> Grab(Uri uri, GrabOptions options)
        {
            // init
            var id = GrabId(uri);
            if (id == null)
                return Array.Empty<IGrabbed>();

            // update to standard Instagram link
            uri = MakeStandardInstagramUri(id);

            // download target page
            Status.Update(null, "Downloading page...", WorkStatusType.DownloadingPage);
            var client = HttpHelper.CreateClient(uri);
            var response = await client.GetAsync(uri);

            // check response
            CheckResponse(response);

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                // parse page
                var meta = ParsePage(responseStream);

                // parse pairs
                return GrabUsingMetadata(meta);
            }
        }
        #endregion
    }
}
