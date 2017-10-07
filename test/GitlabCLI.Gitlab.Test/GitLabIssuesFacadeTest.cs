using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitLabApiClient.Models.Issues.Responses;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Utilities;
using Xunit;
using static GitLabCLI.GitLab.Test.GitLabApiHelper;

namespace GitLabCLI.GitLab.Test
{
    public sealed class GitLabIssuesFacadeTest
    {
        private readonly GitLabIssuesFacade _sut = new GitLabIssuesFacade(ClientFactory);

        [Fact]
        public async Task CreatesIssue()
        {
            var result = await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                ProjectName,
                CurrentUser)
            {
                Labels = new[] { "label1", "label2" }
            });

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                result.Value,
                i => i.Title == "title1" &&
                    i.Description == "description1" &&
                    i.Assignee.Username == CurrentUser &&
                    i.Labels.SequenceEqual(new[] { "label1", "label2" }) &&
                    i.State == IssueState.Opened);
        }

        [Fact]
        public async Task CreatesIssueForCurrentUser()
        {
            var result = await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                ProjectName)
            {
                AssignedToCurrentUser = true
            });

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                result.Value,
                i => i.Assignee.Username == CurrentUser);
        }

        [Fact]
        public async Task CreateIssueForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.CreateIssue(new CreateIssueParameters(
                "title1",
                "description1",
                NonExistingProjectName)
            {
                AssignedToCurrentUser = true
            });

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListsIssuesForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.ListIssues(new ListIssuesParameters(
                NonExistingProjectName, 
                CurrentUser));

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
                ProjectName,
                CurrentUser));

            //act
            var result = await _sut.ListIssues(new ListIssuesParameters(ProjectName, CurrentUser));

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
                ProjectName,
                CurrentUser)
            {
                Labels = randomIssueLabels
            });

            //act
            var result = await _sut.ListIssues(new ListIssuesParameters(ProjectName, CurrentUser)
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
                ProjectName,
                CurrentUser));

            //act
            var result = await _sut.ListIssues(new ListIssuesParameters(ProjectName)
            {
                AssignedToCurrentUser = true
            });

            //assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(i => i.Title == randomIssueTitle);
        }
    }
}
