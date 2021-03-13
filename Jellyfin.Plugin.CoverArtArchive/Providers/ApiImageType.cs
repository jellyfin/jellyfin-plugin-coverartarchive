namespace Jellyfin.Plugin.CoverArtArchive.Providers
{
    /// <summary>
    /// The api image type.
    /// </summary>
    /// <remarks>
    /// https://musicbrainz.org/doc/Cover_Art/Types.
    /// </remarks>
    public enum ApiImageType
    {
        /// <summary>
        /// Front image.
        /// </summary>
        /// <remarks>
        /// ImageType.Box
        /// </remarks>
        Front,

        /// <summary>
        /// Back image.
        /// </summary>
        /// <remarks>
        /// ImageType.BoxRear
        /// </remarks>
        Back, // ImageType.BoxRear

        /// <summary>
        /// Booklet image.
        /// </summary>
        Booklet,

        /// <summary>
        /// Medium image
        /// </summary>
        /// <remarks>
        /// ImageType.Disc
        /// </remarks>
        Medium, // ImageType.Disc

        /// <summary>
        /// Tray image.
        /// </summary>
        Tray,

        /// <summary>
        /// Obi image.
        /// </summary>
        Obi,

        /// <summary>
        /// Spine image.
        /// </summary>
        Spine,

        /// <summary>
        /// Track image.
        /// </summary>
        Track,

        /// <summary>
        /// Liner image.
        /// </summary>
        Liner,

        /// <summary>
        /// Sticker image.
        /// </summary>
        Sticker,

        /// <summary>
        /// Poster image
        /// </summary>
        /// <remarks>
        /// ImageType.Art
        /// </remarks>
        Poster, // ImageType.Art

        /// <summary>
        /// Watermark image.
        /// </summary>
        Watermark,

        /// <summary>
        /// Other image
        /// </summary>
        /// <remarks>
        /// Raw or Unedited
        /// </remarks>
        Other,
    }
}