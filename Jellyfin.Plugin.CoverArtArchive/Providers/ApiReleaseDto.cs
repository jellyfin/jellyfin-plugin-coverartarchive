using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.CoverArtArchive.Providers
{
    /// <summary>
    /// The api release dto.
    /// </summary>
    public class ApiReleaseDto
    {
        /// <summary>
        /// Gets or sets the release.
        /// </summary>
        public string? Release { get; set; }

        /// <summary>
        /// Gets or sets the list of images.
        /// </summary>
        public IReadOnlyList<ApiImageDto> Images { get; set; } = Array.Empty<ApiImageDto>();
    }
}