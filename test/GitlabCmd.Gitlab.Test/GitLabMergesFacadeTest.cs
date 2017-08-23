using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Core.Gitlab.Merges;
using Xunit;
using static GitlabCmd.Gitlab.Test.GitLabApiHelper;
using MergeRequestState = GitlabCmd.Core.Gitlab.Merges.MergeRequestState;

namespace GitlabCmd.Gitlab.Test
{
    public class GitLabFacadeMergeRequestsTest : IAsyncLifetime
    {
        private readonly GitlabMergesFacade _sut = new GitlabMergesFacade(
            new GitLabClientExFactory(new GitLabSettings
            {
                GitLabAccessToken = "KZKSRcxxHi82r4D4p_aJ",
                GitLabHostUrl = "https://gitlab.com/api/v3"
            }), 
            new Mapper());

        [Fact]
        public async Task CreatesMergeRequest()
        {
            string randomTitle = $"title{Guid.NewGuid()}";

            var result = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                randomTitle,
                "develop",
                "master",
                ProjectName,
                CurrentUser));

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

            var result = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                randomTitle,
                "develop",
                "master",
                ProjectName)
            {
                AssignedToCurrentUser = true
            });

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
            var result = await _sut.CreateMergeRequest(new CreateMergeRequestParameters(
                "title1", 
                "develop", 
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
                "develop",
                "master",
                ProjectName));

            //act
            var openedRequests = await _sut.ListMergeRequests(new ListMergesParameters(
                ProjectName,
                MergeRequestState.Opened));

            openedRequests.Value.Should().ContainSingle(s => s.Id == mergeRequest.Value);

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
                "develop",
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

            openedRequests.Value.Should().ContainSingle(s => s.Id == mergeRequest.Value);
        }

        public async Task DisposeAsync() => await DeleteAllMergeRequests(ProjectName);

        public Task InitializeAsync() => Task.CompletedTask;
    }
}
