using System;
using Jellyfin.Plugin.CoverArtArchive.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.CoverArtArchive
{
    /// <summary>
    /// The cover art plugin.
    /// </summary>
    public class CoverArtArchivePlugin : BasePlugin<PluginConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoverArtArchivePlugin"/> class.
        /// </summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
        public CoverArtArchivePlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        /// <inheritdoc />
        public override string Name => "Cover Art Archive";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("8119f3c6-cfc2-4d9c-a0ba-028f1d93e526");

        /// <summary>
        /// Gets the plugin instance.
        /// </summary>
        public static CoverArtArchivePlugin? Instance { get; private set; }
    }
}
