using GitLabCmd.Core;
using GitLabCmd.Utilities;

namespace GitLabCmd.Console.Configuration
{
    public sealed class AppSettingsValidator
    {
        private readonly AppSettings _settings;

        public AppSettingsValidator(AppSettings settings) => _settings = settings;

        public Result Validate()
        {
            if (!ValidateHostUrl())
                return Result.Fail("GitLab host url is not set");

            if (!ValidateAuthorizationSettings())
                return Result.Fail("GitLab authorization options are not set");

            return Result.Ok();
        }

        private bool ValidateHostUrl() => !string.IsNullOrEmpty(_settings.GitLabHostUrl);

        private bool ValidateAuthorizationSettings()
        {
            bool tokenExits = _settings.GitLabAccessToken.IsNotNullOrEmpty();
            bool credentialsExits = _settings.GitLabUserName.IsNotNullOrEmpty() || 
                _settings.GitLabPassword.IsNotNullOrEmpty();

            return tokenExits || credentialsExits;
        }
    }
}
