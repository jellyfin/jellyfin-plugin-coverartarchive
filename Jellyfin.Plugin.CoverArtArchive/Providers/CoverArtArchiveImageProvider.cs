using System;
using System.Collections.Generic;
using System.IO;
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

namespace Jellyfin.Plugin.CoverArtArchive.Providers {

    /* https://musicbrainz.org/doc/Cover_Art/Types */
    public enum ApiImageTypeEnum {
        Front, // ImageType.Box
        Back, // ImageType.BoxRear
        Booklet,
        Medium, // ImageType.Disc
        Tray,
        Obi,
        Spine,
        Track,
        Liner,
        Sticker,
        Poster, // ImageType.Art
        Watermark,
        // Raw/Unedited,
        Other,
    }

    public class ApiRelease {
        public string Release { get; set; }
        public List<ApiImage> Images { get; set; }
    }

    public class ApiImage {
        public List<ApiImageTypeEnum> Types { get; set; }
        public bool Front { get; set; }
        public bool Back { get; set; }
        public string Image { get; set; }
        public ApiThumbnails Thumbnails  { get; set; }
        public string Comment { get; set; }
        public bool Approved { get; set; }

    }

    public class ApiThumbnails {
        public string Small { get; set; }
        public string Large { get; set; }
    }

    public class CoverArtArchiveImageProvider : IRemoteImageProvider {
        private readonly ILogger<CoverArtArchiveImageProvider> _logger;
        private readonly IHttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;
        public CoverArtArchiveImageProvider(IHttpClient httpClient, ILogger<CoverArtArchiveImageProvider> logger) {
            _httpClient = httpClient;
            _logger = logger;

            _serializerOptions = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
            };
            _serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        public string Name => "Cover Art Archive";

        public bool Supports(BaseItem item) => item is MusicAlbum;

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item) {
            return new[] { ImageType.Primary, ImageType.Box, ImageType.BoxRear, ImageType.Disc };
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken) {
            _logger.LogDebug("GetImageResponse({url})", url);
            return _httpClient.GetResponse(new HttpRequestOptions {
                UserAgent = Constants.UserAgent,
                CancellationToken = cancellationToken,
                Url = url
            });
        }

        private async Task<IEnumerable<RemoteImageInfo>> _getImages(string url, CancellationToken cancellationToken) {
            _logger.LogDebug("_getImages({url})", url);
            List<RemoteImageInfo> list = new List<RemoteImageInfo>();

            var response = await _httpClient.SendAsync(
                new HttpRequestOptions {
                    Url = url,
                },
                HttpMethod.Get
            );
            if (response.StatusCode == HttpStatusCode.OK) {
                ApiRelease release = await JsonSerializer.DeserializeAsync<ApiRelease>(response.Content, _serializerOptions);

                foreach (ApiImage image in release.Images) {
                    _logger.LogDebug(image.Types.ToString());
                    if (image.Types.Contains(ApiImageTypeEnum.Front)) {
                        list.Add(
                            new RemoteImageInfo {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.Box,
                                ThumbnailUrl = image.Thumbnails.Small ?? image.Thumbnails.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            }
                        );
                        list.Add(
                            new RemoteImageInfo {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.Primary,
                                ThumbnailUrl = image.Thumbnails.Small ?? image.Thumbnails.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            }
                        );
                    }
                    if (image.Types.Contains(ApiImageTypeEnum.Back)) {
                        list.Add(
                            new RemoteImageInfo {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.BoxRear,
                                ThumbnailUrl = image.Thumbnails.Small ?? image.Thumbnails.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            }
                        );
                    }
                    if (image.Types.Contains(ApiImageTypeEnum.Medium)) {
                        list.Add(
                            new RemoteImageInfo {
                                ProviderName = Name,
                                Url = image.Image,
                                Type = ImageType.Disc,
                                ThumbnailUrl = image.Thumbnails.Small ?? image.Thumbnails.Large,
                                CommunityRating = image.Approved ? 1 : 0,
                                RatingType = RatingType.Score,
                            }
                        );
                    }
                }
            } else {
                _logger.LogWarning("Got HTTP {} - {}", response.StatusCode, response.ResponseUrl);
            }
            return list;
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken) {
            var list = new List<RemoteImageInfo>();
            var musicBrainzId = item.GetProviderId(MetadataProvider.MusicBrainzAlbum);

            if (!string.IsNullOrEmpty(musicBrainzId)) {
                list.AddRange(await _getImages($"{Constants.ApiBaseUrl}/release/{musicBrainzId}/", cancellationToken));
            }
            if (list.Count == 0) {
                var musicBrainzGroupId = item.GetProviderId(MetadataProvider.MusicBrainzReleaseGroup);
                if (!string.IsNullOrEmpty(musicBrainzGroupId)) {
                    list.AddRange(await _getImages($"{Constants.ApiBaseUrl}/release-group/{musicBrainzGroupId}/", cancellationToken));
                }
            }
            return list;
        }

    }
}
