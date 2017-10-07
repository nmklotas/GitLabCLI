using System.IO;
using GitLabCLI.Utilities;
using Newtonsoft.Json;

namespace GitLabCLI.Console.Configuration
{
    public sealed class AppSettingsStorage
    {
        private readonly JsonSerializer _serializer;
        private readonly string _settingsFile;
        private AppSettings _settings;

        public AppSettingsStorage(JsonSerializer serializer, string settingsFile)
        {
            _serializer = serializer;
            _settingsFile = settingsFile;
        }

        public AppSettings Load()
        {
            if (_settings != null)
                return _settings;

            EnsureSettingsDirectoryExists();

            if (!File.Exists(_settingsFile))
            {
                _settings = new AppSettings();
                return _settings;
            }

            using (var file = File.OpenText(_settingsFile))
            using (var reader = new JsonTextReader(file))
                _settings = _serializer.Deserialize<AppSettings>(reader) ?? new AppSettings();

            return _settings;
        }

        public void Save(AppSettings settings)
        {
            EnsureSettingsDirectoryExists();

            using (var fs = File.OpenWrite(_settingsFile))
            using (var sw = new StreamWriter(fs))
            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                _serializer.Serialize(jw, settings);
            }

            _settings = settings;
        }

        private void EnsureSettingsDirectoryExists()
        {
            string settingsDirectory = Path.GetDirectoryName(_settingsFile);
            if (settingsDirectory.IsNullOrEmpty())
                return;

            Directory.CreateDirectory(settingsDirectory);
        }
    }
}
