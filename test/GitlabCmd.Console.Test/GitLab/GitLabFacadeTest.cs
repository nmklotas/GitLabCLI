using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.GitLab;
using Xunit;
using static GitlabCmd.Console.Test.GitLab.GitLabApiHelper;

namespace GitlabCmd.Console.Test.GitLab
{
    public class GitLabFacadeTest
    {
        private readonly GitLabFacade _sut = new GitLabFacade(
            new Lazy<GitLabClientEx>(Client));

        [Fact]
        public async Task AddIssueCreatesIssue()
        {
            var result = await _sut.AddIssue(
                "title1", "description1", 
                ProjectName, 
                UserName,
                new[] {"label1", "label2"});

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                ProjectName,
                result.Value, issue =>
                {
                    issue.Title.Should().Be("title1");
                    issue.Description.Should().Be("description1");
                    issue.Assignee.Username.Should().Be(UserName);
                    issue.Labels.Should().BeEquivalentTo("label1", "label2");
                    issue.State.Should().BeEquivalentTo("opened");
                });
        }

        [Fact]
        public async Task AddIssueForCurrentUserCreatesIssueForCurrentUser()
        {
            var result = await _sut.AddIssueForCurrentUser(
                "title1", 
                "description1",
                ProjectName);

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                ProjectName,
                result.Value, issue =>
                {
                    issue.Assignee.Username.Should().Be(UserName);
                });
        }

        [Fact]
        public async Task AddIssueForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.AddIssueForCurrentUser(
                "title1",
                "description1",
                NonExistingProjectName);

            result.IsSuccess.Should().BeFalse();
        }
    }
}
