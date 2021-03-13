using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.CoverArtArchive.Providers
{
    /// <summary>
    /// The cover art archive image provider.
    /// </summary>
    public class CoverArtArchiveImageProvider : IRemoteImageProvider
    {
        private readonly ILogger<CoverArtArchiveImageProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverArtArchiveImageProvider"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Instance of the <see cref="IHttpClientFactory"/> interface.</param>
        /// <param name="logger">Instance of the <see cref="ILogger{CoverArtArchiveImageProvider}"/> interface.</param>
        public CoverArtArchiveImageProvider(
            IHttpClientFactory httpClientFactory,
            ILogger<CoverArtArchiveImageProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        /// <inheritdoc />
        public string Name
            => "Cover Art Archive";

        /// <inheritdoc />
        public bool Supports(BaseItem item)
            => item is MusicAlbum;

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[]
            {
                ImageType.Primary,
                ImageType.Box,
                ImageType.BoxRear,
                ImageType.Disc
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.LogDebug("GetImageResponse({Url})", url);
            var httpClient = _httpClientFactory.CreateClient(NamedClient.Default);
            return await httpClient.GetAsync(new Uri(url), cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var list = new List<RemoteImageInfo>();
            var musicBrainzId = item.GetProviderId(MetadataProvider.MusicBrainzAlbum);

            if (!string.IsNullOrEmpty(musicBrainzId))
            {
                list.AddRange(await GetImagesInternal($"{Constants.ApiBaseUrl}/release/{musicBrainzId}/", cancellationToken)
                    .ConfigureAwait(false));
            }

            if (list.Count == 0)
            {
                var musicBrainzGroupId = item.GetProviderId(MetadataProvider.MusicBrainzReleaseGroup);
                if (!string.IsNullOrEmpty(musicBrainzGroupId))
                {
                    list.AddRange(await GetImagesInternal($"{Constants.ApiBaseUrl}/release-group/{musicBrainzGroupId}/", cancellationToken)
                        .ConfigureAwait(false));
                }
            }

            return list;
        }

        private async Task<IEnumerable<RemoteImageInfo>> GetImagesInternal(string url, CancellationToken cancellationToken)
        {
            _logger.LogDebug("GetImagesInternal({Url})", url);
            List<RemoteImageInfo> list = new List<RemoteImageInfo>();

            var httpClient = _httpClientFactory.CreateClient(NamedClient.Default);
            var response = await httpClient.GetAsync(new Uri(url), cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                var releaseDto = await JsonSerializer.DeserializeAsync<ApiReleaseDto>(stream, _serializerOptions, cancellationToken)
                    .ConfigureAwait(false);

                if (releaseDto == null)
                {
                    return Array.Empty<RemoteImageInfo>();
                }

                foreach (ApiImageDto image in releaseDto.Images)
                {
                    _logger.LogDebug("ImageType: {ImageType}", image.Types);
                    if (image.Types.Contains(ApiImageType.Front))
                    {
                        list.Add(
                            new RemoteImageInfo
                            {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.Box,
                                ThumbnailUrl = image.Thumbnails?.Small ?? image.Thumbnails?.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            });
                        list.Add(
                            new RemoteImageInfo
                            {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.Primary,
                                ThumbnailUrl = image.Thumbnails?.Small ?? image.Thumbnails?.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            });
                    }

                    if (image.Types.Contains(ApiImageType.Back))
                    {
                        list.Add(
                            new RemoteImageInfo
                            {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.BoxRear,
                                ThumbnailUrl = image.Thumbnails?.Small ?? image.Thumbnails?.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            });
                    }

                    if (image.Types.Contains(ApiImageType.Medium))
                    {
                        list.Add(
                            new RemoteImageInfo
                            {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.Disc,
                                ThumbnailUrl = image.Thumbnails?.Small ?? image.Thumbnails?.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            });
                    }
                }
            }
            else
            {
                _logger.LogWarning("Got HTTP {StatusCode} - {Location}", response.StatusCode, response.Headers.Location);
            }

            return list;
        }
    }
}
