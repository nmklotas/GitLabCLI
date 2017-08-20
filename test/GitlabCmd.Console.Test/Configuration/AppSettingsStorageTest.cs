using System.IO;
using FluentAssertions;
using GitlabCmd.Console.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace GitlabCmd.Console.Test.Configuration
{
    public class AppSettingsStorageTest
    {
        [Fact]
        public void SavedSettingsCanBeLoaded()
        {
            var sut = new AppSettingsStorage(
                JsonSerializer.CreateDefault(), 
                Path.GetRandomFileName());

            sut.Save(new AppSettings
            {
                GitLabHostUrl = "testhost",
                GitLabUserName = "testusername",
                GitLabPassword = "testpassword",
                GitLabAccessToken = "testtoken",
                DefaultProject = "testproject",
                DefaultIssuesProject = "testdefaultissuesproject",
                DefaultMergesProject = "testdefaultmergesproject",
                DefaulIssuesLabel = "testdefaultissueslabel"
            });

            sut.Load().Should().Match<AppSettings>(s =>
                s.GitLabHostUrl == "testhost" &&
                s.GitLabUserName == "testusername" &&
                s.GitLabPassword == "testpassword" &&
                s.GitLabAccessToken == "testtoken" &&
                s.DefaultProject == "testproject" &&
                s.DefaultIssuesProject == "testdefaultissuesproject" &&
                s.DefaultMergesProject == "testdefaultmergesproject" &&
                s.DefaulIssuesLabel == "testdefaultissueslabel");
        }
    }
}
