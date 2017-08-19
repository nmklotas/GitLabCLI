using FluentAssertions;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Cmd;
using GitlabCmd.Console.Configuration;
using Xunit;

namespace GitlabCmd.Console.Test.App
{
    public class ParametersHandlerTest
    {
        private readonly ParametersHandler _sut;

        public ParametersHandlerTest()
        {
            var settings = new AppSettings
            {
                DefaultGitLabProject = "test-project",
                DefaulGitLabIssueLabel = "test-issue-label",
                GitLabAccessToken = "test-access-token",
                GitLabHostUrl = "https://test-gitlab.com"
            };

            _sut = new ParametersHandler(settings);
        }

        [Fact]
        public void NotProvidedProjectNameTakenFromSettings()
        {
            var parameter = _sut.NegotiateAddIssueParameters(new CreateIssueOptions
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
