using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DotNetTools.SharpGrabber.Exceptions;
using DotNetTools.SharpGrabber.YouTube.YouTube;
using DotNetTools.SharpGrabber.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetTools.SharpGrabber.YouTube
{
    /// <summary>
    /// Default <see cref="IGrabber"/> for YouTube
    /// </summary>
    public class YouTubeGrabber : YouTubeGrabberBase
    {
        #region Compiled Regular Expressions
        private static readonly Regex BaseJsLocatorRegex = new Regex(@"<script[^<>]+src=""([^""]+base\.js)""", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex InnerTubeApiKeyRegex = new Regex(@"INNERTUBE_API_KEY""\s*:\s*""([^""]+)""", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex DecipherFunctionRegex = new Regex(@"\s(\w+)=function\S+split\([^\n\r]+join\(\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        #endregion

        #region Internal Methods => Metadata
        /// <summary>
        /// URL-encoded data are randomly mangled by YouTube. Some entries are merged with other ones separated by comma.
        /// This method iterates through the mangled entries and invokes <paramref name="feedCallback"/> for each real
        /// independent entry. Finally, <paramref name="feedCallback"/> is called supplied with NULL arguments to enforce
        /// a final commit.
        /// </summary>
        protected virtual void UnmangleEntries(List<KeyValuePair<string, string>> entries, Action<string, string> feedCallback)
        {
            // iterate through list
            var inString = false;
            var sb = new StringBuilder();
            foreach (var pair in entries)
            {
                sb.Clear();
                var currentKey = pair.Key;
                foreach (var ch in pair.Value.Concat(new[] { '\0' }))
                {
                    switch (ch)
                    {
                        case '"':
                            inString = !inString;
                            continue;

                        case ',':
                        case '=':
                        case '\0':
                            if (inString)
                                break;

                            // skip if we see equal sign and we already have a key
                            if (ch == '=' && currentKey != null)
                                break;

                            if (currentKey == null)
                                currentKey = sb.ToString();
                            else
                            {
                                feedCallback(currentKey, sb.ToString());
                                currentKey = null;
                            }

                            sb.Clear();
                            continue;
                    }

                    sb.Append(ch);
                }
            }

            feedCallback(null, null);
        }

        /// <summary>
        /// Gets invoked by <see cref="DownloadMetadata"/> to extract muxed streams from metadata.
        /// </summary>
        protected virtual List<YouTubeMuxedStream> ExtractMuxedStreamsMetadata(List<KeyValuePair<string, string>> fmtStreamMapEntries)
        {
            var list = new List<YouTubeMuxedStream>();
            var propertySet = new HashSet<string>();
            var draft = new YouTubeMuxedStream();

            void CommitDraft()
            {
                if (draft?.iTag != null && draft.Url != null)
                    list.Add(draft);
                propertySet.Clear();
                draft = new YouTubeMuxedStream();
            }

            void Feed(string key, string value)
            {
                // check for force commit
                if (key == null)
                {
                    CommitDraft();
                    return;
                }

                // check for rotation
                if (propertySet.Contains(key))
                    CommitDraft();
                propertySet.Add(key);

                // update draft
                switch (key.ToLowerInvariant())
                {
                    case "itag":
                        draft.iTag = int.Parse(value);
                        break;

                    case "type":
                        draft.Type = value;
                        var parts = value.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        draft.Mime = parts[0];
                        break;

                    case "quality":
                        draft.Quality = value;
                        break;

                    case "url":
                        draft.Url = value;
                        break;

                    case "s":
                        draft.Signature = value;
                        break;
                }
            }

            UnmangleEntries(fmtStreamMapEntries, Feed);
            return list;
        }

        /// <summary>
        /// Gets invoked by <see cref="DownloadMetadata"/> to extract adaptive_fmts.
        /// </summary>
        protected virtual List<YouTubeAdaptiveStream> ExtractAdaptiveFormatsMetadata(List<KeyValuePair<string, string>> adaptiveFmts)
        {
            var list = new List<YouTubeAdaptiveStream>();
            var propertySet = new HashSet<string>();
            var draft = new YouTubeAdaptiveStream();

            void CommitDraft()
            {
                if (draft?.iTag != null && draft.Url != null)
                    list.Add(draft);
                propertySet.Clear();
                draft = new YouTubeAdaptiveStream();
            }

            void Feed(string key, string value)
            {
                // check for force commit
                if (key == null)
                {
                    CommitDraft();
                    return;
                }

                // check for rotation
                if (propertySet.Contains(key))
                    CommitDraft();
                propertySet.Add(key);

                // update draft
                switch (key.ToLowerInvariant())
                {
                    case "quality_label":
                        draft.QualityLabel = value;
                        break;

                    case "itag":
                        draft.iTag = int.Parse(value);
                        break;

                    case "fps":
                        draft.FPS = int.Parse(value);
                        break;

                    case "bitrate":
                        draft.AudioSampleRate = int.Parse(value) * 1000;
                        break;

                    case "type":
                        draft.Type = value;
                        var parts = value.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        draft.Mime = parts[0];
                        break;

                    case "size":
                        var sizeRegex = new Regex(@"([0-9]+)[^0-9]+([0-9]+)");
                        var match = sizeRegex.Match(value);
                        if (!match.Success)
                            throw new GrabParseException($"Failed to parse stream size: {value}.");
                        draft.FrameSize = new Size(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                        break;

                    case "url":
                        draft.Url = value;
                        break;

                    case "s":
                        draft.Signature = value;
                        break;
                }
            }

            UnmangleEntries(adaptiveFmts, Feed);
            return list;
        }

        /// <summary>
        /// Accepts <paramref name="cipher"/> as a URI-encoded string in the following form:
        /// <para>sp=sig&amp;s=wggj6zg7m-...&amp;url=https%3A%2F%2Fr5---sn-5hnekn7k.googlevideo.com%2Fvideoplayback...</para>
        /// Extracts its useful encoded parameters and puts them into the specified <paramref name="streamInfo"/>.
        /// </summary>
        protected virtual void UpdateStreamCipherInfo(YouTubeStreamInfo streamInfo, string cipher)
        {
            if (string.IsNullOrEmpty(cipher))
                throw new ArgumentNullException(nameof(cipher));

            var map = YouTubeUtils.ExtractUrlEncodedParamMap(cipher);
            streamInfo.Url = map.GetOrDefault("url") ?? throw new GrabParseException("Failed to extract URL from cipher.");
            streamInfo.Signature = map.GetOrDefault("s") ?? throw new GrabParseException("Failed to extract signature from cipher.");
        }

        /// <summary>
        /// Given the content type, extracts its pure mime type. For instance, accepts 'video/webm;+codecs=vp9' and returns 'video/webm'. 
        /// </summary>
        protected virtual string ExtractActualMime(string contentType)
        {
            var re = new Regex(@"^([^;]+)(;|$)");
            var match = re.Match(contentType);
            if (!match.Success)
                throw new GrabParseException("Failed to extract mime information of muxed stream type.");
            return match.Groups[1].Value;
        }

        /// <summary>
        /// Translates the given JSON object extracted from YouTube to its managed representation.
        /// </summary>
        protected virtual YouTubeMuxedStream TranslateMuxedStream(JObject input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new YouTubeMuxedStream
            {
                iTag = input.Value<int?>("itag"),
                Type = input.Value<string>("mimeType"),
                AudioSampleRate = input.Value<long>("audioSampleRate"),
                ContentLength = input.Value<long>("contentLength"),
                Quality = input.Value<string>("quality"),
                QualityLabel = input.Value<string>("qualityLabel"),
                Url = input.Value<string>("url"),
                FrameSize = input.ContainsKey("width") && input.ContainsKey("height")
                    ? new Size(input.Value<int>("width"), input.Value<int>("height"))
                    : null
            };
            result.Mime = ExtractActualMime(result.Type);

            // get cipher info (+signatureCipher)
            var cipher = input.Value<string>("cipher") ?? input.Value<string>("signatureCipher");
            if (!string.IsNullOrEmpty(cipher))
                UpdateStreamCipherInfo(result, cipher);

            return result;
        }

        /// <summary>
        /// Translates the given JSON object extracted from YouTube to its managed representation.
        /// </summary>
        protected virtual YouTubeAdaptiveStream TranslateAdaptiveStream(JObject input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var result = new YouTubeAdaptiveStream
            {
                iTag = input.Value<int?>("itag"),
                Type = input.Value<string>("mimeType"),
                AudioSampleRate = input.Value<long>("audioSampleRate"),
                ContentLength = input.Value<long>("contentLength"),
                FPS = input.Value<int>("fps"),
                Quality = input.Value<string>("quality"),
                QualityLabel = input.Value<string>("qualityLabel"),
                Url = input.Value<string>("url"),
                FrameSize = input.ContainsKey("width") && input.ContainsKey("height")
                    ? new Size(input.Value<int>("width"), input.Value<int>("height"))
                    : null
            };
            result.Mime = ExtractActualMime(result.Type);

            // get cipher info
            var cipher = input.Value<string>("cipher") ?? input.Value<string>("signatureCipher");
            if (!string.IsNullOrEmpty(cipher))
                UpdateStreamCipherInfo(result, cipher);

            return result;
        }

        /// <summary>
        /// Given the specified JSON array, returns a list of multiplexed streams.
        /// </summary>
        protected List<YouTubeMuxedStream> TranslateMuxedStreams(JArray array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            return array.Children<JObject>().Select(TranslateMuxedStream).ToList();
        }

        /// <summary>
        /// Given the specified JSON array, returns a list of adaptive streams.
        /// </summary>
        protected List<YouTubeAdaptiveStream> TranslateAdaptiveStreams(JArray array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            return array.Children<JObject>().Select(TranslateAdaptiveStream).ToList();
        }

        /// <summary>
        /// Given player_response JSON data, returns its .NET representation.
        /// </summary>
        protected virtual YouTubePlayerResponse ExtractPlayerResponseMetadata(JToken playerResponse)
        {
            var statusString = playerResponse.SelectToken("$.playabilityStatus.status").Value<string>();
            if (!statusString.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                throw new GrabException("YouTube video is inaccessible.");

            // get video details
            var videoDetails = playerResponse.SelectToken("$.videoDetails");
            var microformat = playerResponse.SelectToken("$.microformat");
            var captions = playerResponse.SelectToken("$.captions");

            // prepare result
            var result = new YouTubePlayerResponse
            {
                Author = StringHelper.DecodeUriString(videoDetails.SelectToken("author").Value<string>()),
                Length = TimeSpan.FromSeconds(videoDetails.SelectToken("lengthSeconds").Value<int>()),
                Title = StringHelper.DecodeUriString(videoDetails.SelectToken("title").Value<string>()),
                AverageRating = videoDetails.SelectToken("averageRating").Value<double>(),
                ChannelId = videoDetails.SelectToken("channelId").Value<string>(),
                ShortDescription = StringHelper.DecodeUriString(videoDetails.SelectToken("shortDescription").Value<string>()),
                ViewCount = videoDetails.SelectToken("viewCount").Value<long>(),
                UploadedAt = microformat?.SelectToken("..uploadDate").Value<DateTime>(),
                PublishedAt = microformat?.SelectToken("..publishDate").Value<DateTime>(),
            };

            // get streaming data
            if (playerResponse.SelectToken("$.streamingData") is JObject streamingData)
            {
                var muxedFormats = streamingData.Value<JArray>("formats");
                var adaptiveFormats = streamingData.Value<JArray>("adaptiveFormats");
                if (muxedFormats != null)
                    result.MuxedStreams = TranslateMuxedStreams(muxedFormats);
                if (adaptiveFormats != null)
                    result.AdaptiveStreams = TranslateAdaptiveStreams(adaptiveFormats);
            }

            return result;
        }

        /// <summary>
        /// Downloads metadata for the YouTube video with the specified ID.
        /// </summary>
        protected virtual async Task<YouTubeMetadata> DownloadMetadata(string id, YouTubeEmbedPageData embedPageData,
            CancellationToken cancellationToken)
        {
            IDictionary<string, string> rawMetadata;
            Status.Update(null, "Downloading metadata...", WorkStatusType.DownloadingFile);

            // make http client
            var client = HttpHelper.GetClient();

            // send http request
            using var response = await GetYouTubeVideoInfoResponse(client, id, embedPageData.Key, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new GrabException($"Failed to get media info.");

            // decode metadata into rawMetadata
            var content = await response.Content.ReadAsStringAsync();
            rawMetadata = YouTubeUtils.ExtractUrlEncodedParamMap(content);

            // extract metadata
            var metadata = new YouTubeMetadata
            {
                FormatList = rawMetadata.ContainsKey("fmt_list") ? rawMetadata["fmt_list"] : null,
                Status = rawMetadata.ContainsKey("status") ? rawMetadata["status"] : null,
            };

            // extract player response
            //var rawPlayerResponse = rawMetadata["player_response"]
            //                        ?? throw new GrabParseException("Failed to fetch player_response from metadata.");
            //var playerResponse = JToken.Parse(rawPlayerResponse);
            var playerResponse = JToken.Parse(content);
            metadata.PlayerResponse = ExtractPlayerResponseMetadata(playerResponse);

            // extract muxed streams
            if (metadata.PlayerResponse.MuxedStreams != null)
                metadata.MuxedStreams = metadata.PlayerResponse.MuxedStreams;
            if (metadata.MuxedStreams == null)
            {
                var urlEncodedFmtStreamMap = rawMetadata.GetOrDefault("url_encoded_fmt_stream_map")
                                             ?? throw new GrabParseException("Failed to fetch url_encoded_fmt_stream_map from metadata.");
                var fmtStreamMap = YouTubeUtils.ExtractUrlEncodedParamList(urlEncodedFmtStreamMap);
                metadata.MuxedStreams = ExtractMuxedStreamsMetadata(fmtStreamMap);
            }

            // extract adaptive streams
            if (metadata.PlayerResponse.AdaptiveStreams != null)
                metadata.AdaptiveStreams = metadata.PlayerResponse.AdaptiveStreams;
            if (metadata.AdaptiveStreams == null)
            {
                var urlEncodedAdaptiveFormats = rawMetadata["adaptive_fmts"]
                                                ?? throw new GrabParseException("Failed to fetch adaptive_fmts from metadata.");
                var adaptiveFmts = YouTubeUtils.ExtractUrlEncodedParamList(urlEncodedAdaptiveFormats);
                metadata.AdaptiveStreams = ExtractAdaptiveFormatsMetadata(adaptiveFmts);
            }

            return metadata;
        }
        #endregion

        #region Internal Methods => Deciphering
        /// <summary>
        /// Invoked by <see cref="GrabAsync"/> method when the target video has a signature.
        /// This method downloads the necessary script (base.js) and decipher all grabbed links. 
        /// </summary>
        protected virtual async Task Decipher(YouTubeEmbedPageData embedPageData, YouTubeMetadata metaData, CancellationToken cancellationToken)
        {
            // download base.js
            var client = HttpHelper.GetClient();
            using var response = await client.GetAsync(embedPageData.BaseJsUri, cancellationToken);
            var scriptContent = await response.Content.ReadAsStringAsync();
            var script = new YouTubeScript(scriptContent);

            // find decipher function name
            var match = DecipherFunctionRegex.Match(scriptContent);
            if (!match.Success)
                throw new GrabParseException("Failed to locate decipher function.");
            var fn = match.Groups[1].Value;

            // prepare script host to execute the decipher function along with its used functions
            script.PrepareDecipherFunctionCall(fn);

            // call decipher function
            foreach (var streamInfo in metaData.AllStreams)
            {
                if (string.IsNullOrEmpty(streamInfo.Signature))
                    continue;

                // calculate decipher
                streamInfo.Decipher = script.CallDecipherFunction(fn, streamInfo.Signature);

                // update uri
                streamInfo.Url += $"&sig={Uri.EscapeDataString(streamInfo.Decipher)}";
            }
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Downloads video's embed page and extracts useful data.
        /// </summary>
        protected virtual async Task<YouTubeEmbedPageData> DownloadEmbedPage(string id)
        {
            var result = new YouTubeEmbedPageData();

            // get embed uri
            var embedUri = GetYouTubeEmbedUri(id);

            // download embed page
            var client = HttpHelper.GetClient();
            var embedPageContent = await client.GetStringAsync(embedUri);

            // find base.js
            var match = BaseJsLocatorRegex.Match(embedPageContent);
            if (!match.Success)
                throw new GrabParseException("Failed to find base.js script reference.");
            result.BaseJsUri = new Uri(embedUri, match.Groups[1].Value);

            // find inner tube api key
            match = InnerTubeApiKeyRegex.Match(embedPageContent);
            if (!match.Success)
                throw new GrabParseException("Failed to locate INNERTUBE_API_KEY.");
            result.Key = match.Groups[1].Value;
            return result;
        }

        /// <summary>
        /// Generates all possible image URLs of the specified YouTube video.
        /// </summary>
        protected virtual void AppendImagesToResult(GrabResult result, string id, bool useHttps = true)
        {
            // We're gonna iterate through all possible image types and add links to every image
            // into result resources. Notice that since these URIs are not checked by sending HTTP
            // requests, there is a rare possibility for some generated links to be invalid and
            // missing from YouTube servers.

            var imageTypes = Enum.GetValues(typeof(YouTubeImageType));
            foreach (YouTubeImageType imageType in imageTypes)
            {
                var uri = GetYouTubeImageUri(id, imageType, useHttps);
                var img = new GrabbedImage(GrabbedImageType.Primary, null, uri);
                result.Resources.Add(img);
            }
        }

        /// <summary>
        /// Updates information about the grabbed media according to the given YouTube iTag info.
        /// </summary>
        protected virtual void UpdateStreamITagInfo(GrabbedMedia grabbed, YouTubeTagInfo itag)
        {
            grabbed.BitRateString = itag.BitRateString;
            grabbed.Container = itag.Container;
            grabbed.Resolution = itag.VideoResolution;
            grabbed.FormatId = itag.iTag;
            if (!string.IsNullOrEmpty(grabbed.Container))
                grabbed.Format.Extension = grabbed.Container.ToLowerInvariant();
            var attributes = new List<string>
            {
                grabbed.Container?.ToUpperInvariant(),
                grabbed.Resolution
            };
            if (grabbed.Channels != MediaChannels.Both)
                attributes.Add($"({grabbed.Channels} only)");
            grabbed.FormatTitle = string.Join(" ", attributes.Where(attr => !string.IsNullOrWhiteSpace(attr)));
        }

        /// <summary>
        /// Appends the specified <paramref name="stream"/> to the specified <paramref name="result"/>.
        /// </summary>
        protected virtual void AppendStreamToResult(GrabResult result, YouTubeStreamInfo stream)
        {
            MediaChannels channels;

            // get iTag info
            var itagInfo = stream.iTag == null ? null : YouTubeTags.For(stream.iTag.Value);

            // extract extension from mime
            var extension = stream.Extension ?? stream.Mime?.Split('/')?.Last();

            // decide according to stream type - adaptive, or muxed
            if (stream is YouTubeMuxedStream muxedStream)
            {
                // Muxed stream
                channels = MediaChannels.Both;
            }
            else if (stream is YouTubeAdaptiveStream adaptiveStream)
            {
                // Adaptive stream
                var hasVideo = itagInfo?.HasVideo ?? stream.Mime.StartsWith("video");
                channels = hasVideo ? MediaChannels.Video : MediaChannels.Audio;
            }
            else
                throw new NotSupportedException($"YouTube stream of type {stream.GetType()} is not implemented in {nameof(YouTubeGrabber)}.{nameof(AppendStreamToResult)}.");

            var format = new MediaFormat(stream.Mime, extension);
            var grabbed = new GrabbedMedia(new Uri(stream.Url), null, format, channels);
            result.Resources.Add(grabbed);

            // update grabbed media iTag info
            if (itagInfo != null)
                UpdateStreamITagInfo(grabbed, itagInfo.Value);
        }

        /// <summary>
        /// Updates <see cref="GrabResult"/> according to the information obtained from metadata.
        /// </summary>
        private void UpdateResult(GrabResult result, YouTubeMetadata md)
        {
            var response = md.PlayerResponse;
            result.Title = response.Title;
            result.CreationDate = response.UploadedAt;
            result.Description = response.ShortDescription;

            result.Statistics = new GrabStatisticInfo
            {
                Author = response.Author,
                Length = response.Length,
                ViewCount = response.ViewCount
            };
        }
        #endregion

        #region Grab Method
        /// <inheritdoc />
        protected override async Task GrabAsync(GrabResult result, string id, CancellationToken cancellationToken, GrabOptions options,
            IProgress<double> progress)
        {
            // extract base.js script
            var embedPageData = await DownloadEmbedPage(id);

            // download metadata
            var metaData = await DownloadMetadata(id, embedPageData, cancellationToken);

            // update result according to the metadata
            UpdateResult(result, metaData);

            // are there any encrypted streams?
            result.IsSecure = metaData.AllStreams.Any(stream => !string.IsNullOrEmpty(stream.Signature));

            // should we decipher the signature?
            if (result.IsSecure && options.Flags.HasFlag(GrabOptionFlags.Decipher))
                await Decipher(embedPageData, metaData, cancellationToken);

            // append images to the result
            if (options.Flags.HasFlag(GrabOptionFlags.GrabImages))
                AppendImagesToResult(result, id);

            // append muxed streams to result
            foreach (var stream in metaData.MuxedStreams)
                AppendStreamToResult(result, stream);

            // append adaptive streams to result
            foreach (var stream in metaData.AdaptiveStreams)
                AppendStreamToResult(result, stream);
        }
        #endregion
    }
}