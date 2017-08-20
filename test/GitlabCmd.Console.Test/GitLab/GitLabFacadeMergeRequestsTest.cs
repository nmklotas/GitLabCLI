using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
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
        public async Task CreateMergeRequestCreatesMergeRequest()
        {
            string randomTitle = $"title{Guid.NewGuid()}";

            var result = await _sut.CreateMergeRequest(
                ProjectName,
                randomTitle,
                "develop",
                "master",
                UserName);

            result.IsSuccess.Should().BeTrue();

            await ShouldHaveMergeRequest(
                ProjectName,
                result.Value, issue =>
                {
                    issue.Title.Should().Be(randomTitle);
                    issue.Assignee.Username.Should().Be(UserName);
                    issue.SourceBranch.Should().Be("develop");
                    issue.TargetBranch.Should().Be("master");
                    issue.State.Should().BeEquivalentTo("opened");
                });
        }

        [Fact]
        public async Task CreateMergeRequestForCurrentUserCreatesMergeRequestForCurrentUser()
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
                result.Value, issue =>
                {
                    issue.Title.Should().Be(randomTitle);
                    issue.Assignee.Username.Should().Be(UserName);
                    issue.SourceBranch.Should().Be("develop");
                    issue.TargetBranch.Should().Be("master");
                    issue.State.Should().BeEquivalentTo("opened");
                });
        }

        [Fact]
        public async Task CreateMergeRequestForNonExistingProjectReturnsFailedResult()
        {
            var result = await _sut.CreateMergeRequest(
                NonExistingProjectName, "title1", "develop", "master");

            result.IsSuccess.Should().BeFalse();
        }

        public async Task DisposeAsync() => await DeleteAllMergeRequests(ProjectName);

        public Task InitializeAsync() => Task.CompletedTask;
    }
}
