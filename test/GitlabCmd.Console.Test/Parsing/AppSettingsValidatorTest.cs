using System;
using FluentAssertions;
using GitLabCLI.Console.Configuration;
using Xunit;

namespace GitLabCLI.Console.Test.Parsing
{
    public sealed class AppSettingsValidatorTest
    {
        private readonly Func<AppSettings, AppSettingsValidator> _sut = s => new AppSettingsValidator(s);

        [Fact]
        public void SettingsWithoutHostUrlAreNotValid()
        {
            var appSettings = new AppSettings
            {
                DefaultProject = "test-project",
                GitLabAccessToken = "access-token",
                DefaulIssuesLabel = "parsed-issue-label"
            };

            _sut(appSettings).Validate().IsSuccess.Should().BeFalse();
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

            _sut(appSettings).Validate().IsSuccess.Should().BeFalse();
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

            _sut(appSettings).Validate().IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void SettingsWithTokenAuthorizationAreValid()
        {
            var appSettings = new AppSettings
            {
                GitLabHostUrl = "https://test.com",
                GitLabAccessToken = "test-token"
            };

            _sut(appSettings).Validate().IsSuccess.Should().BeTrue();
        }
    }
}
