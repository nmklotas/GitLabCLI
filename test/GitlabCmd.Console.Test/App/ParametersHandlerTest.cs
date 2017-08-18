using FluentAssertions;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Cmd;
using GitlabCmd.Console.Configuration;
using Xunit;

namespace GitlabCmd.Console.Test.App
{
    public class ParametersHandlerTest
    {
        private readonly AppSettings _settings;
        private readonly ParametersHandler _sut;

        public ParametersHandlerTest()
        {
            _settings = new AppSettings
            {
                DefaultGitLabProject = "test-project",
                DefaultIssueLabel = "test-issue-label",
                GitLabAccessToken = "test-access-token",
                GitLabHostUrl = "https://test-gitlab.com"
            };

            _sut = new ParametersHandler(_settings);
        }

        [Fact]
        public void NotProvidedProjectNameTakenFromSettings()
        {
            var parameter = _sut.GetAddIssueParameters(new AddIssueOptions
            {
                Description = "parsed-description",
                Labels = new[] { "parsed-label" },
                Title = "parsed-title"
            });

            parameter.IsSuccess.Should().BeTrue();
            parameter.Value.ProjectName.Should().Be("test-project");
        }
    }
}
