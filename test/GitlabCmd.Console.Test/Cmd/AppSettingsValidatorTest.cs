using System;
using FluentAssertions;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Configuration;
using Xunit;

namespace GitlabCmd.Console.Test.Cmd
{
    public class AppSettingsValidatorTest
    {
        private readonly Func<AppSettings, AppSettingsValidator> _sutFactory =
            s => new AppSettingsValidator(s, new OutputPresenter(new GridResultFormatter()));

        [Fact]
        public void SettingsWithoutHostUrlAreNotValid()
        {
            var appSettings = new AppSettings
            {
                DefaultProject = "test-project",
                GitLabAccessToken = "access-token",
                DefaulIssuesLabel = "parsed-issue-label"
            };

            _sutFactory(appSettings).Validate().Should().BeFalse();
        }

        [Fact]
        public void SettingsWithoutAuthorizationSetAreNotValid()
        {
            var appSettings = new AppSettings
            {
                GitLabHostUrl = "https://test.com",
                DefaultProject = "test-project",
                DefaulIssuesLabel = "parsed-issue-label"
            };

            _sutFactory(appSettings).Validate().Should().BeFalse();
        }

        [Fact]
        public void SettingsWithUsernameAndPasswordAuthorizationAreValid()
        {
            var appSettings = new AppSettings
            {
                GitLabHostUrl = "https://test.com",
                GitLabUserName = "username",
                GitLabPassword = "password"
            };

            _sutFactory(appSettings).Validate().Should().BeTrue();
        }

        [Fact]
        public void SettingsWithTokenAuthorizationAreValid()
        {
            var appSettings = new AppSettings
            {
                GitLabHostUrl = "https://test.com",
                GitLabAccessToken = "test-token"
            };

            _sutFactory(appSettings).Validate().Should().BeTrue();
        }
    }
}
