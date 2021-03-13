namespace Jellyfin.Plugin.CoverArtArchive.Providers
{
    /// <summary>
    /// Api thumbnails dto.
    /// </summary>
    public class ApiThumbnailsDto
    {
        /// <summary>
        /// Gets or sets the small thumbnail.
        /// </summary>
        public string? Small { get; set; }

        /// <summary>
        /// Gets or sets the large thumbnail.
        /// </summary>
        public string? Large { get; set; }
    }
}