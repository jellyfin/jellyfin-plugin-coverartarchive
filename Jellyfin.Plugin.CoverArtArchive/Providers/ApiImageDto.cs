using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.CoverArtArchive.Providers
{
    /// <summary>
    /// The api image dto.
    /// </summary>
    public class ApiImageDto
    {
        /// <summary>
        /// Gets or sets the list of types.
        /// </summary>
        public IReadOnlyList<ApiImageType> Types { get; set; } = Array.Empty<ApiImageType>();

        /// <summary>
        /// Gets or sets a value indicating whether this is a front image.
        /// </summary>
        public bool Front { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a back image.
        /// </summary>
        public bool Back { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Gets or sets the thumbnails.
        /// </summary>
        public ApiThumbnailsDto? Thumbnails { get; set; }

        /// <summary>
        /// Gets or sets the image comment.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this image has been approved.
        /// </summary>
        public bool Approved { get; set; }
    }
}