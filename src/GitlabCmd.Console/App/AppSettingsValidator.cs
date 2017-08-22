using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.Utilities;

namespace GitlabCmd.Console.App
{
    public sealed class AppSettingsValidator
    {
        private readonly AppSettings _settings;
        private readonly OutputPresenter _outputPresenter;

        public AppSettingsValidator(
            AppSettings settings, 
            OutputPresenter outputPresenter)
        {
            _settings = settings;
            _outputPresenter = outputPresenter;
        }

        public bool Validate()
        {
            if (!ValidateHostUrl())
                return false;

            if (!ValidateAuthorizationSettings())
                return false;

            return true;
        }

        private bool ValidateHostUrl()
        {
            if (string.IsNullOrEmpty(_settings.GitLabHostUrl))
            {
                _outputPresenter.Info("GitLab host url is not set");
                return false;
            }

            return true;
        }

        private bool ValidateAuthorizationSettings()
        {
            bool tokenExits = _settings.GitLabAccessToken.IsNotEmpty();
            bool credentialsExits = _settings.GitLabUserName.IsNotEmpty() || 
                _settings.GitLabPassword.IsNotEmpty();
            if (tokenExits || credentialsExits)
            {
                return true;
            }

            _outputPresenter.Info("GitLab authorization options are not set");
            return false;
        }
    }
}
