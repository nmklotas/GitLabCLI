using System.Linq;
using System.Reflection;
using GitLabCLI.Console.Output;
using GitLabCLI.Core;
using Newtonsoft.Json;

namespace GitLabCLI.Console.Configuration
{
    public sealed class ConfigurationHandler
    {
        private readonly AppSettingsStorage _storage;
        private readonly AppSettingsValidator _validator;
        private readonly OutputPresenter _outputPresenter;

        public ConfigurationHandler(
            AppSettingsStorage storage,
            AppSettingsValidator validator,
            OutputPresenter outputPresenter)
        {
            _storage = storage;
            _validator = validator;
            _outputPresenter = outputPresenter;
        }

        public Result Validate() 
            => _validator.Validate();

        public void StoreConfiguration(ConfigurationParameters parameters)
        {
            var settings = _storage.Load();

            if (SaveConfigurationValues(parameters, settings))
                _outputPresenter.ShowMessage("Configuration saved successfully.");

            var encryptedSettings = _storage.LoadWithSensitiveDataEncrypted();
            ShowConfiguration(encryptedSettings);
        }

        private void ShowConfiguration(AppSettings settings)
        {
            _outputPresenter.ShowGrid(
                "Current configuration",
                new GridColumn("Name", 100, GetSettingsProperties(settings).Select(GetPropertyName)),
                new GridColumn("Value", 100, GetSettingsProperties(settings).Select(p => p.GetValue(settings))));
        }

        private static PropertyInfo[] GetSettingsProperties(AppSettings settings)
        {
            return settings.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private static string GetPropertyName(PropertyInfo property)
        {
            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
            return jsonProperty != null ? jsonProperty.PropertyName : property.Name;
        }

        private bool SaveConfigurationValues(
            ConfigurationParameters parameters,
            AppSettings settings)
        {
            bool configurationChanged = false;

            if (parameters.DefaultProject != null)
            {
                settings.DefaultProject = parameters.DefaultProject;
                configurationChanged = true;
            }
            if (parameters.Token != null)
            {
                settings.GitLabAccessToken = parameters.Token;
                configurationChanged = true;
            }
            if (parameters.Host != null)
            {
                settings.GitLabHostUrl = parameters.Host;
                configurationChanged = true;
            }
            if (parameters.Username != null)
            {
                settings.GitLabUserName = parameters.Username;
                configurationChanged = true;
            }
            if (parameters.Password != null)
            {
                settings.GitLabPassword = parameters.Password;
                configurationChanged = true;
            }
            if (parameters.DefaultIssuesProject != null)
            {
                settings.DefaultIssuesProject = parameters.DefaultIssuesProject;
                configurationChanged = true;
            }
            if (parameters.DefaultMergesProject != null)
            {
                settings.DefaultMergesProject = parameters.DefaultMergesProject;
                configurationChanged = true;
            }
            if (parameters.DefaulIssuesLabel != null)
            {
                settings.DefaulIssuesLabel = parameters.DefaulIssuesLabel;
                configurationChanged = true;
            }

            if (configurationChanged)
                _storage.Save(settings);

            return configurationChanged;
        }
    }
}