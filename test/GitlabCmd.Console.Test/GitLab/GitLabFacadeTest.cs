using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
using GitlabCmd.Console.Utilities;
using Xunit;
using static GitlabCmd.Console.Test.GitLab.GitLabApiHelper;

namespace GitlabCmd.Console.Test.GitLab
{
    public class GitLabFacadeTest
    {
        private readonly GitLabFacade _sut = new GitLabFacade(new GitLabClientFactory(new AppSettings
        {
            GitLabAccessToken = "KZKSRcxxHi82r4D4p_aJ",
            GitLabHostUrl = "https://gitlab.com/api/v3"
        }));

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

        [Fact]
        public async Task ListIssuesForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.ListIssues(
                NonExistingProjectName);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListIssuesGetsIssues()
        {
            //arrange
            string randomIssueTitle = $"title{Guid.NewGuid()}";

            await _sut.AddIssue(
                randomIssueTitle,
                "description1",
                ProjectName,
                UserName);

            //act
            var result = await _sut.ListIssues(ProjectName, UserName);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }

        [Fact]
        public async Task ListIssuesGetsFilteredByLabel()
        {
            //arrange
            string[] randomIssueLabels = { $"label{Guid.NewGuid()}" };

            await _sut.AddIssue(
                "title1",
                "description1",
                ProjectName,
                UserName,
                randomIssueLabels);

            //act
            var result = await _sut.ListIssues(ProjectName, UserName, randomIssueLabels);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value.Should().ContainSingle(i => i.Labels.Contains(randomIssueLabels));
        }

        [Fact]
        public async Task ListIssuesForCurrentUser()
        {
            //arrange
            string randomIssueTitle = $"title{Guid.NewGuid()}";

            //arrange
            await _sut.AddIssue(
                randomIssueTitle,
                "description1",
                ProjectName,
                UserName);

            //act
            var result = await _sut.ListIssuesForCurrentUser(ProjectName);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }
    }
}
