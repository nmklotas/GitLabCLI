using GitLabCLI.Console.Output;
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

            settings.DefaultProject = parameters.DefaultProject;
            settings.GitLabAccessToken = parameters.Token;
            settings.GitLabHostUrl = parameters.Host;
            settings.GitLabUserName = parameters.Username;
            settings.GitLabPassword = parameters.Password;
            settings.DefaultIssuesProject = parameters.DefaultIssuesProject;
            settings.DefaultMergesProject = parameters.DefaultMergesProject;
            settings.DefaulIssuesLabel = parameters.DefaulIssuesLabel;

            _storage.Save(settings);
        }
    }
}