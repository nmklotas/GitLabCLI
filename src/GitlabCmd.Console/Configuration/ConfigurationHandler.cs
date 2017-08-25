using GitLabCLI.Core;

namespace GitLabCLI.Console.Configuration
{
    public sealed class ConfigurationHandler
    {
        private readonly AppSettingsStorage _storage;
        private readonly AppSettingsValidator _validator;

        public ConfigurationHandler(
            AppSettingsStorage storage,
            AppSettingsValidator validator)
        {
            _storage = storage;
            _validator = validator;
        }

        public Result Validate() => _validator.Validate();

        public void StoreConfiguration(ConfigurationParameters parameters)
        {
            var settings = _storage.Load();

            if (parameters.DefaultProject != null)
                settings.DefaultProject = parameters.DefaultProject;
            if (parameters.Token != null)
                settings.GitLabAccessToken = parameters.Token;
            if (parameters.Host != null)
                settings.GitLabHostUrl = parameters.Host;
            if (parameters.Username != null)
                settings.GitLabUserName = parameters.Username;
            if (parameters.Password != null)
                settings.GitLabPassword = parameters.Password;
            if (parameters.DefaultIssuesProject != null)
                settings.DefaultIssuesProject = parameters.DefaultIssuesProject;
            if (parameters.DefaultMergesProject != null)
                settings.DefaultMergesProject = parameters.DefaultMergesProject;
            if (parameters.DefaulIssuesLabel != null)
                settings.DefaulIssuesLabel = parameters.DefaulIssuesLabel;

            _storage.Save(settings);
        }
    }
}