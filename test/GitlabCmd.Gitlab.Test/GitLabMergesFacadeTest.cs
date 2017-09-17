using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitLabCLI.Core.Gitlab.Merges;
using Xunit;
using static GitLabCLI.GitLab.Test.GitLabApiHelper;
using MergeRequestState = GitLabCLI.Core.Gitlab.Merges.MergeRequestState;

namespace GitLabCLI.GitLab.Test
{
    public sealed class GitLabFacadeMergeRequestsTest : IAsyncLifetime
    {
        private readonly GitLabMergesFacade _sut = new GitLabMergesFacade(ClientFactory, new Mapper());

        [Fact]
        public async Task CreatesMergeRequest()
        {
            string randomTitle = $"title{Guid.NewGuid()}";

            var result = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                randomTitle,
                "feature",
                "master",
                ProjectName,
                CurrentUser));

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveMergeRequest(
                result.Value,
                m => m.Title == randomTitle &&
                     m.Assignee.Username == CurrentUser &&
                     m.SourceBranch == "feature" &&
                     m.TargetBranch == "master" &&
                     m.State == GitLabApiClient.Models.MergeRequests.Responses.MergeRequestState.Opened);
        }

        [Fact]
        public async Task CreatesMergeRequestForCurrentUser()
        {
            string randomTitle = $"title{Guid.NewGuid()}";

            var result = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                randomTitle,
                "feature",
                "master",
                ProjectName)
            {
                AssignedToCurrentUser = true
            });

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveMergeRequest(
                result.Value, 
                m => m.Title == randomTitle &&
                    m.Assignee.Username == CurrentUser &&
                    m.SourceBranch == "feature" &&
                    m.TargetBranch == "master" &&
                    m.State == GitLabApiClient.Models.MergeRequests.Responses.MergeRequestState.Opened);
        }

        [Fact]
        public async Task CreateMergeRequestForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                "title1",
                "feature", 
                "master", 
                NonExistingProjectName));

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ListsAvailableMergeRequests()
        {
            //arrange
            var mergeRequest = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                "title1",
                "feature",
                "master",
                ProjectName));

            //act
            var openedRequests = await _sut.ListMergeRequests(new ListMergesParameters(
                ProjectName,
                MergeRequestState.Opened));

            openedRequests.Value.Should().ContainSingle(s => s.Iid == mergeRequest.Value);

            var closedRequests = await _sut.ListMergeRequests(new ListMergesParameters(
                ProjectName,
                MergeRequestState.Closed));

            closedRequests.Value.Should().BeEmpty();

            var mergedRequests = await _sut.ListMergeRequests(new ListMergesParameters(
                ProjectName, 
                MergeRequestState.Merged));

            mergedRequests.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task ListsAvailableMergeRequestsForCurrentUser()
        {
            //arrange
            var mergeRequest = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                "title1",
                "feature",
                "master",
                ProjectName,
                CurrentUser));

            //act
            var openedRequests = await _sut.ListMergeRequests(new ListMergesParameters(
                ProjectName,
                MergeRequestState.Opened)
            {
                AssignedToCurrentUser = true
            });

            openedRequests.Value.Should().ContainSingle(s => s.Iid == mergeRequest.Value);
        }

        public Task DisposeAsync() => DeleteAllMergeRequests();

        public Task InitializeAsync() => DeleteAllMergeRequests();
    }
}
