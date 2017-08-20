using System.IO;
using Newtonsoft.Json;

namespace GitlabCmd.Console.Configuration
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

            if (!File.Exists(_settingsFile))
                _settings = new AppSettings();

            using (var file = File.OpenText(_settingsFile))
            using (var reader = new JsonTextReader(file))
                _settings = _serializer.Deserialize<AppSettings>(reader) ?? new AppSettings();

            return _settings;
        }

        public void Save(AppSettings settings)
        {
            using (var fs = File.OpenWrite(_settingsFile))
            using (var sw = new StreamWriter(fs))
            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                _serializer.Serialize(jw, settings);
            }
        }
    }
}
