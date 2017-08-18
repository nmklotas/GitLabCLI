using GitlabCmd.Console.Configuration;

namespace GitlabCmd.Console.App
{
    public class AppSettingsValidationHandler
    {
        private readonly AppSettings _settings;
        private readonly OutputPresenter _outputPresenter;

        public AppSettingsValidationHandler(
            AppSettings settings, 
            OutputPresenter outputPresenter)
        {
            _settings = settings;
            _outputPresenter = outputPresenter;
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(_settings.GitLabHostUrl))
            {
                _outputPresenter.Info("GitLabHostUrl is not set");
                return false;
            }

            return true;
        }
    }
}
