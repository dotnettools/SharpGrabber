using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.BlackWidow.Repository.GitHub
{
    /// <summary>
    /// Defines a grabber repository that fetches the feed file and the scripts from a GitHub repository.
    /// </summary>
    public class GitHubGrabberRepository : GrabberRepositoryBase
    {
        private readonly HttpClient _client;
        private readonly bool _ownClient;

        public GitHubGrabberRepository(HttpClient httpClient, bool ownClient = true)
        {
            _ownClient = ownClient;
            _client = httpClient;
            _client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            {
                NoCache = true,
                MaxAge = TimeSpan.Zero,
            };
        }

        public GitHubGrabberRepository() : this(new HttpClient()) { }

        public override bool CanPut => false;

        /// <summary>
        /// Gets or sets the name of the repository e.g. 'dotnettools/SharpGrabber'
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets the branch name.
        /// </summary>
        public string BranchName { get; set; } = "master";

        /// <summary>
        /// Gets or sets the path to the directory that contains the feed file and the scripts.
        /// </summary>
        public string RepoRootPath { get; set; } = "blackwidow";

        /// <summary>
        /// Gets or sets the name of the feed file.
        /// </summary>
        public string FeedFileName { get; set; } = "feed.json";

        public override async Task<IGrabberRepositoryFeed> GetFeedAsync(CancellationToken cancellationToken)
        {
            var url = GetFeedUrl();
            using var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var feedModel = JsonConvert.DeserializeObject<FeedFileModel>(content);
            return new GrabberRepositoryFeed(feedModel.Scripts);
        }

        public override async Task<IGrabberScriptSource> FetchSourceAsync(IGrabberRepositoryScript _script, CancellationToken cancellationToken)
        {
            if (_script is not FeedScriptModel script)
                throw new InvalidOperationException($"The provided script does not belong to this repository.");

            var url = GetScriptUrl(script.File);
            using var response = await _client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var src = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return new GrabberScriptSource(src);
        }

        public override Task PutAsync(IGrabberRepositoryScript script, IGrabberScriptSource source, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("Putting is not supported for GitHub repositories.");
        }

        protected override void Dispose(bool disposing)
        {
            if (_ownClient)
                _client.Dispose();
        }

        protected virtual string GetFeedUrl()
            => GetRawUrl(BranchName, RepoRootPath, FeedFileName);

        protected virtual string GetScriptUrl(string fileName)
            => GetRawUrl(BranchName, RepoRootPath, fileName);

        protected virtual string GetRawUrl(params string[] parts)
            => ($"https://raw.githubusercontent.com/{Repository}" + '/' + string.Join("/", parts ?? Array.Empty<string>()).Trim('/')).Trim('/');

        private sealed class FeedFileModel
        {
            public FeedScriptModel[] Scripts { get; set; }
        }

        private sealed class FeedScriptModel : GrabberRepositoryScript
        {
            public string File { get; set; }
        }
    }
}
