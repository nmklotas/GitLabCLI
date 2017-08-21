using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
using NGitLab.Models;
using Xunit;
using static GitlabCmd.Console.Test.GitLab.GitLabApiHelper;

namespace GitlabCmd.Console.Test.GitLab
{
    public class GitLabFacadeMergeRequestsTest : IAsyncLifetime
    {
        private readonly GitLabFacade _sut = new GitLabFacade(new GitLabClientFactory(new AppSettings
        {
            GitLabAccessToken = "KZKSRcxxHi82r4D4p_aJ",
            GitLabHostUrl = "https://gitlab.com/api/v3"
        }));

        [Fact]
        public async Task CreatesMergeRequest()
        {
            string randomTitle = $"title{Guid.NewGuid()}";

            var result = await _sut.CreateMergeRequest(
                ProjectName,
                randomTitle,
                "develop",
                "master",
                CurrentUser);

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveMergeRequest(
                ProjectName,
                result.Value,
                m => m.Title == randomTitle &&
                     m.Assignee.Username == CurrentUser &&
                     m.SourceBranch == "develop" &&
                     m.TargetBranch == "master" &&
                     m.State == "opened");
        }

        [Fact]
        public async Task CreatesMergeRequestForCurrentUser()
        {
            string randomTitle = $"title{Guid.NewGuid()}";

            var result = await _sut.CreateMergeRequestForCurrentUser(
                ProjectName,
                randomTitle,
                "develop",
                "master");

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveMergeRequest(
                ProjectName,
                result.Value, 
                m => m.Title == randomTitle &&
                    m.Assignee.Username == CurrentUser &&
                    m.SourceBranch == "develop" &&
                    m.TargetBranch == "master" &&
                    m.State == "opened");
        }

        [Fact]
        public async Task CreateMergeRequestForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.CreateMergeRequest(
                NonExistingProjectName, "title1", "develop", "master");

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListsAvailableMergeRequests()
        {
            //arrange
            var mergeRequest = await _sut.CreateMergeRequest(
                ProjectName,
                "title1",
                "develop",
                "master");

            //act
            var openedRequests = await _sut.ListMergeRequests(ProjectName, MergeRequestState.opened);
            openedRequests.Value.Should().ContainSingle(s => s.Id == mergeRequest.Value);

            var closedRequests = await _sut.ListMergeRequests(ProjectName, MergeRequestState.closed);
            closedRequests.Value.Should().BeEmpty();

            var mergedRequests = await _sut.ListMergeRequests(ProjectName, MergeRequestState.merged);
            mergedRequests.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task ListsAvailableMergeRequestsForCurrentUser()
        {
            //arrange
            var mergeRequest = await _sut.CreateMergeRequest(
                ProjectName,
                "title1",
                "develop",
                "master",
                CurrentUser);

            //act
            var openedRequests = await _sut.ListMergeRequests(ProjectName, MergeRequestState.opened, CurrentUser);
            openedRequests.Value.Should().ContainSingle(s => s.Id == mergeRequest.Value);
        }

        public async Task DisposeAsync() => await DeleteAllMergeRequests(ProjectName);

        public Task InitializeAsync() => Task.CompletedTask;
    }
}
