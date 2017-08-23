using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Utilities;
using Xunit;
using static GitlabCmd.Gitlab.Test.GitLabApiHelper;

namespace GitlabCmd.Gitlab.Test
{
    public class GitLabIssuesFacadeTest
    {
        private readonly GitlabIssuesFacade _sut = new GitlabIssuesFacade(new GitLabClientExFactory(new GitLabSettings
        {
            GitLabAccessToken = "KZKSRcxxHi82r4D4p_aJ",
            GitLabHostUrl = "https://gitlab.com/api/v3"
        }));

        [Fact]
        public async Task CreatesIssue()
        {
            var result = await _sut.CreateIssue(
                "title1", "description1",
                ProjectName,
                CurrentUser,
                new[] { "label1", "label2" });

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                ProjectName,
                result.Value, 
                i => i.Title == "title1" &&
                    i.Description == "description1" &&
                    i.Assignee.Username == CurrentUser &&
                    i.Labels.SequenceEqual(new [] { "label1", "label2" }) &&
                    i.State == "opened");
        }

        [Fact]
        public async Task CreatesIssueForCurrentUser()
        {
            var result = await _sut.CreateIssueForCurrentUser(
                "title1",
                "description1",
                ProjectName);

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                ProjectName,
                result.Value, 
                i => i.Assignee.Username == CurrentUser);
        }

        [Fact]
        public async Task CreateIssueForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.CreateIssueForCurrentUser(
                "title1",
                "description1",
                NonExistingProjectName);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListsIssuesForNonExistingProjectReturnsFailedResult()
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

            await _sut.CreateIssue(
                randomIssueTitle,
                "description1",
                ProjectName,
                CurrentUser);

            //act
            var result = await _sut.ListIssues(ProjectName, CurrentUser);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }

        [Fact]
        public async Task ListsIssuesFilteredByLabel()
        {
            //arrange
            string[] randomIssueLabels = { $"label{Guid.NewGuid()}" };

            await _sut.CreateIssue(
                "title1",
                "description1",
                ProjectName,
                CurrentUser,
                randomIssueLabels);

            //act
            var result = await _sut.ListIssues(ProjectName, CurrentUser, randomIssueLabels);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value.Should().ContainSingle(i => i.Labels.Contains(randomIssueLabels));
        }

        [Fact]
        public async Task ListsIssuesForCurrentUser()
        {
            //arrange
            string randomIssueTitle = $"title{Guid.NewGuid()}";

            //arrange
            await _sut.CreateIssue(
                randomIssueTitle,
                "description1",
                ProjectName,
                CurrentUser);

            //act
            var result = await _sut.ListIssuesForCurrentUser(ProjectName);

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }
    }
}
