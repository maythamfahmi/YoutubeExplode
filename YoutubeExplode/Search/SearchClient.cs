using System;
using System.Collections.Generic;
using System.Net.Http;
using YoutubeExplode.Common;
using YoutubeExplode.ReverseEngineering.Responses;
using YoutubeExplode.Videos;

namespace YoutubeExplode.Search
{
    /// <summary>
    /// YouTube search queries.
    /// </summary>
    public class SearchClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes an instance of <see cref="SearchClient"/>.
        /// </summary>
        public SearchClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Enumerates videos returned by the specified search query.
        /// </summary>
        public async IAsyncEnumerable<Video> GetVideosAsync(string searchQuery)
        {
            var encounteredVideoIds = new HashSet<string>();

            for (var page = 0; page < int.MaxValue; page++)
            {
                var response = await PlaylistResponse.GetSearchResultsAsync(_httpClient, searchQuery, page);

                var countDelta = 0;
                foreach (var video in response.GetVideos())
                {
                    var videoId = video.GetId();

                    // Yield the video if it's distinct
                    if (!encounteredVideoIds.Add(videoId))
                        continue;

                    yield return new Video(
                        videoId,
                        video.GetTitle(),
                        video.GetAuthor(),
                        video.GetUploadDate(),
                        video.GetDescription(),
                        video.GetDuration(),
                        Array.Empty<Thumbnail>(),
                        video.GetKeywords(),
                        new Engagement(
                            video.GetViewCount(),
                            video.GetLikeCount(),
                            video.GetDislikeCount()
                        ));

                    countDelta++;
                }

                // Videos loop around, so break when we stop seeing new videos
                if (countDelta <= 0)
                    break;
            }
        }
    }
}