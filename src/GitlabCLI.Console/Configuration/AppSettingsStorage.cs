using System.IO;
using System.Security.Cryptography;
using GitLabCLI.Console.Parameters;
using GitLabCLI.Utilities;
using Newtonsoft.Json;

namespace GitLabCLI.Console.Configuration
{
    public sealed class AppSettingsStorage
    {
        private readonly JsonSerializer _serializer;
        private readonly string _settingsFile;
        private readonly Encryptor _encryptor;

        public AppSettingsStorage(JsonSerializer serializer, string settingsFile, Encryptor encryptor)
        {
            _serializer = serializer;
            _settingsFile = settingsFile;
            _encryptor = encryptor;
        }

        public AppSettings LoadWithSensitiveDataEncrypted() 
            => LoadSettingsFromFile();

        public AppSettings Load()
        {
            var settings = LoadSettingsFromFile();
            DecryptSensitiveData(settings);
            return settings;
        }

        private AppSettings LoadSettingsFromFile()
        {
            EnsureSettingsDirectoryExists();

            if (!File.Exists(_settingsFile))
                return new AppSettings();

            using (var settingsStream = File.OpenText(_settingsFile))
            using (var textReader = new JsonTextReader(settingsStream))
            {
                return _serializer.Deserialize<AppSettings>(textReader) ?? new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            EnsureSettingsDirectoryExists();

            using (var settingsStream = File.OpenWrite(_settingsFile))
            using (var streamWriter = new StreamWriter(settingsStream))
            using (var textWriter = new JsonTextWriter(streamWriter))
            {
                textWriter.Formatting = Formatting.Indented;
                SerializeSettings(textWriter, settings);
            }
        }

        private void SerializeSettings(JsonTextWriter textWriter, AppSettings settings)
        {
            var settingsClone = settings.Clone();
            EncryptSensitiveData(settingsClone);
            _serializer.Serialize(textWriter, settingsClone);
        }

        private void EncryptSensitiveData(AppSettings settings)
        {
            if (!settings.GitLabPassword.IsNullOrEmpty())
                settings.GitLabPassword = _encryptor.Encrypt(settings.GitLabPassword);

            if (!settings.GitLabAccessToken.IsNullOrEmpty())
                settings.GitLabAccessToken = _encryptor.Encrypt(settings.GitLabAccessToken);
        }

        private void DecryptSensitiveData(AppSettings settings)
        {
            if (!settings.GitLabPassword.IsNullOrEmpty())
            {
                SafeDecryptPassword();
            }
            if (!settings.GitLabAccessToken.IsNullOrEmpty())
            {
                SafeDecryptToken();
            }

            void SafeDecryptPassword()
            {
                try
                {
                    settings.GitLabPassword = _encryptor.Decrypt(settings.GitLabPassword);
                }
                catch (CryptographicException)
                {
                    //if we can't decrypt it, reset it
                    settings.GitLabPassword = null;
                }
            }

            void SafeDecryptToken()
            {
                try
                {
                    settings.GitLabAccessToken = _encryptor.Decrypt(settings.GitLabAccessToken);
                }
                catch (CryptographicException)
                {
                    //if we can't decrypt it, reset it
                    settings.GitLabAccessToken = null;
                }
            }
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
