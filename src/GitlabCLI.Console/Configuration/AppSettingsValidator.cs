using GitLabCLI.Core;
using GitLabCLI.Utilities;

namespace GitLabCLI.Console.Configuration
{
    public sealed class AppSettingsValidator
    {
        private readonly AppSettings _settings;

        public AppSettingsValidator(AppSettings settings) => _settings = settings;

        public Result Validate()
        {
            if (!ValidateHostUrl())
                return Result.Fail("GitLab host url is not set. " +
                                   "Run 'gitlab config --host {host}' to set host url.");

            if (!ValidateAuthorizationSettings())
                return Result.Fail("GitLab authentication options are not set.\r\n" +
                                   "You can set authentication options two ways:\r\n" +
                                   "1. Run 'gitlab config --token {token}' if you have auth token.\r\n" +
                                   "2. Run 'gitlab config --username {username} --password {password}' " +
                                   "if you want to use username and password.\r\n");

            return Result.Ok();
        }

        private bool ValidateHostUrl() => !string.IsNullOrEmpty(_settings.GitLabHostUrl);

        private bool ValidateAuthorizationSettings()
        {
            bool tokenExits = !_settings.GitLabAccessToken.IsNullOrEmpty();
            bool credentialsExits = !_settings.GitLabUserName.IsNullOrEmpty() || 
                !_settings.GitLabPassword.IsNullOrEmpty();

            return tokenExits || credentialsExits;
        }
    }
}
