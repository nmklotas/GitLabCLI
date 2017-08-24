using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Utilities;
using Xunit;

namespace GitLabCLI.GitLab.Test
{
    public sealed class GitLabIssuesFacadeTest
    {
        private readonly GitLabIssuesFacade _sut = new GitLabIssuesFacade(new GitLabClientExFactory(new GitLabSettings
        {
            GitLabAccessToken = "KZKSRcxxHi82r4D4p_aJ",
            GitLabHostUrl = "https://gitlab.com/api/v3"
        }));

        [Fact]
        public async Task CreatesIssue()
        {
            var result = await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                GitLabApiHelper.ProjectName,
                GitLabApiHelper.CurrentUser)
            {
                Labels = new[] { "label1", "label2" }
            });

            result.IsSuccess.Should().BeTrue();

            await GitLabApiHelper.ShouldHaveIssue(
                GitLabApiHelper.ProjectName,
                result.Value,
                i => i.Title == "title1" &&
                    i.Description == "description1" &&
                    i.Assignee.Username == GitLabApiHelper.CurrentUser &&
                    Enumerable.SequenceEqual(i.Labels, new[] { "label1", "label2" }) &&
                    i.State == "opened");
        }

        [Fact]
        public async Task CreatesIssueForCurrentUser()
        {
            var result = await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                GitLabApiHelper.ProjectName)
            {
                AssignedToCurrentUser = true
            });

            result.IsSuccess.Should().BeTrue();

            await GitLabApiHelper.ShouldHaveIssue(
                GitLabApiHelper.ProjectName,
                result.Value,
                i => i.Assignee.Username == GitLabApiHelper.CurrentUser);
        }

        [Fact]
        public async Task CreateIssueForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                GitLabApiHelper.NonExistingProjectName)
            {
                AssignedToCurrentUser = true
            });

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListsIssuesForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.ListIssues(new ListIssuesParameters(
                GitLabApiHelper.NonExistingProjectName, 
                GitLabApiHelper.CurrentUser));

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListIssuesGetsIssues()
        {
            //arrange
            string randomIssueTitle = $"title{Guid.NewGuid()}";

            await _sut.CreateIssue(new CreateIssueParameters(
                randomIssueTitle,
                "description1",
                GitLabApiHelper.ProjectName,
                GitLabApiHelper.CurrentUser));

            //act
            var result = await _sut.ListIssues(new ListIssuesParameters(GitLabApiHelper.ProjectName, GitLabApiHelper.CurrentUser));

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }

        [Fact]
        public async Task ListsIssuesFilteredByLabel()
        {
            //arrange
            string[] randomIssueLabels = { $"label{Guid.NewGuid()}" };

            await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                GitLabApiHelper.ProjectName,
                GitLabApiHelper.CurrentUser)
            {
                Labels = randomIssueLabels
            });

            //act
            var result = await _sut.ListIssues(new ListIssuesParameters(GitLabApiHelper.ProjectName, GitLabApiHelper.CurrentUser)
            {
                Labels = randomIssueLabels
            });

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
            await _sut.CreateIssue(new CreateIssueParameters(
                randomIssueTitle,
                "description1",
                GitLabApiHelper.ProjectName,
                GitLabApiHelper.CurrentUser));

            //act
            var result = await _sut.ListIssues(new ListIssuesParameters(GitLabApiHelper.ProjectName)
            {
                AssignedToCurrentUser = true
            });

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }
    }
}
