using System;
using System.IO;
using FluentAssertions;
using GitLabCLI.Console.Configuration;
using GitLabCLI.Console.Parameters;
using Newtonsoft.Json;
using Xunit;

namespace GitLabCLI.Console.Test.Configuration
{
    public sealed class AppSettingsStorageTest
    {
        private readonly Func<string, AppSettingsStorage> _sut = p => new AppSettingsStorage(
            JsonSerializer.CreateDefault(), p, new Encryptor());

        private readonly AppSettings _settings = new AppSettings
        {
            GitLabHostUrl = "testhost",
            GitLabUserName = "testusername",
            GitLabPassword = "testpassword",
            GitLabAccessToken = "testtoken",
            DefaultProject = "testproject",
            DefaultIssuesProject = "testdefaultissuesproject",
            DefaultMergesProject = "testdefaultmergesproject",
            DefaulIssuesLabel = "testdefaultissueslabel"
        };

        [Fact]
        public void SavedSettingsCanBeLoaded()
        {
            var sut = _sut(Path.GetRandomFileName());
            sut.Save(_settings);
            sut.Load().Should().BeEquivalentTo(_settings);
        }

        [Fact]
        public void SavedSettingsCanBeLoadedFromDisk()
        {
            string fileName = Path.GetRandomFileName();
            _sut(fileName).Save(_settings);
            _sut(fileName).Load().Should().BeEquivalentTo(_settings);
        }
    }
}
