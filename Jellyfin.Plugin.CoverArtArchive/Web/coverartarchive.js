const CoverArtArchiveConfig = {
    pluginUniqueId: '8119f3c6-cfc2-4d9c-a0ba-028f1d93e526'
};

export default function (view, params) {
    view.addEventListener('viewshow', function () {
        Dashboard.showLoadingMsg();
        const page = this;
        ApiClient.getPluginConfiguration(CoverArtArchiveConfig.pluginUniqueId).then(function (config) {
            page.querySelector('#Options').value = config.Options || '';
            page.querySelector('#AnInteger').value = config.AnInteger || '';
            page.querySelector('#TrueFalseSetting').checked = config.TrueFalseSetting;
            page.querySelector('#AString').value = config.AString || '';
            Dashboard.hideLoadingMsg();
        });
    });

    view.querySelector('#CoverArtArchiveConfigForm').addEventListener('submit', function (e) {
        e.preventDefault();
        Dashboard.showLoadingMsg();
        const form = this;
        ApiClient.getPluginConfiguration(CoverArtArchiveConfig.pluginUniqueId).then(function (config) {
            config.Options = form.querySelector('#Options').value;
            config.AnInteger = form.querySelector('#AnInteger').value;
            config.TrueFalseSetting = form.querySelector('#TrueFalseSetting').checked;
            config.AString = form.querySelector('#AString').value;
            ApiClient.updatePluginConfiguration(CoverArtArchiveConfig.pluginUniqueId, config).then(function (result) {
                Dashboard.processPluginConfigurationUpdateResult(result);
            });
        });
        return false;
    });
}
