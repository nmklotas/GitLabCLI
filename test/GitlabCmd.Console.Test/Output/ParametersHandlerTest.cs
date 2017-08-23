using FluentAssertions;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.Parsing;
using Xunit;

namespace GitlabCmd.Console.Test.Output
{
    public class ParametersHandlerTest
    {
        private readonly ParametersHandler _sut;

        public ParametersHandlerTest()
        {
            var settings = new AppSettings
            {
                DefaultProject = "test-project",
                DefaulIssuesLabel = "test-issue-label",
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
            parameter.Value.Project.Should().Be("test-project");
        }
    }
}
