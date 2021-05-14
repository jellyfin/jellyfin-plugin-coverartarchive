using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.CoverArtArchive.Configuration
{
    /// <summary>
    /// Class PluginConfiguration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Gets or sets options.
        /// </summary>
        public string Options { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets an integer.
        /// </summary>
        public int AnInteger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it true or false setting.
        /// </summary>
        public bool TrueFalseSetting { get; set; }

        /// <summary>
        /// Gets or sets a string.
        /// </summary>
        public string AString { get; set; } = string.Empty;
    }
}
