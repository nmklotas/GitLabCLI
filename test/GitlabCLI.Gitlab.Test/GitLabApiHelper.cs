using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using GitLabApiClient;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Responses;

namespace GitLabCLI.GitLab.Test
{
    public static class GitLabApiHelper
    {
        private static readonly GitLabClient _client = 
            new GitLabClient("https://gitlab.com/api/v4", "KZKSRcxxHi82r4D4p_aJ");

        public static GitLabClientFactory ClientFactory { get; } = new GitLabClientFactory(new GitLabSettings
        {
            GitLabAccessToken = "KZKSRcxxHi82r4D4p_aJ",
            GitLabHostUrl = "https://gitlab.com/api/v4"
        });

        public static string ProjectName => "txxxestprojecxxxt";

        public static int ProjectId => 4011625;

        public static string CurrentUser => "nmklotas";

        public static string NonExistingProjectName => Guid.NewGuid().ToString();

        public static async Task ShouldHaveIssue(
            int issueId,
            Expression<Func<Issue, bool>> predicate)
        {
            var issue = await _client.Issues.GetAsync(ProjectId, issueId);
            issue.Should().NotBeNull($"Issue {issueId} does not exists");
            issue.Should().Match(predicate);
        }

        public static async Task ShouldHaveMergeRequest(
            int mergeRequestId,
            Expression<Func<MergeRequest, bool>> predicate)
        {
            var mergeRequests = await _client.MergeRequests.GetAsync(ProjectId);
            var mergeRequest = mergeRequests.FirstOrDefault(s => s.Iid == mergeRequestId);
            mergeRequest.Should().NotBeNull($"Merge request {mergeRequestId} does not exists");
            mergeRequest.Should().Match(predicate);
        }

        public static async Task DeleteAllMergeRequests()
        {
            var mergeRequests = await _client.MergeRequests.GetAsync(ProjectId);
            await Task.WhenAll(mergeRequests.Select(
                m => _client.MergeRequests.DeleteAsync(ProjectId, m.Iid)));
        }
    }
}
