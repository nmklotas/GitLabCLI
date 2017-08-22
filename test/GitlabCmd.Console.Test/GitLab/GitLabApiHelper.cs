using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.GitLab;
using GitlabCmd.Utilities;
using NGitLab.Models;

namespace GitlabCmd.Console.Test.GitLab
{
    public static class GitLabApiHelper
    {
        private static readonly GitLabClientEx _client = 
            new GitLabClientEx("https://gitlab.com/api/v3", "KZKSRcxxHi82r4D4p_aJ");

        public static string ProjectName => "testproject";

        public static string CurrentUser => _client.Users.Current.Username;

        public static string NonExistingProjectName => Guid.NewGuid().ToString();

        public static async Task ShouldHaveIssue(
            string projectName, 
            int issueId,
            Expression<Func<Issue, bool>> predicate)
        {
            var projects = await _client.Projects.Accessible();

            var project = projects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName)) ??
                throw new InvalidOperationException($"project {projectName} does not exists");

            var issue = await _client.Issues.GetAsync(project.Id, issueId);
            issue.Should().NotBeNull($"Issue {issueId} does not exists");
            issue.Should().Match(predicate);
        }

        public static async Task ShouldHaveMergeRequest(
            string projectName,
            int mergeRequestId,
            Expression<Func<MergeRequest, bool>> predicate)
        {
            var projects = await _client.Projects.Accessible();

            var project = projects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName)) ??
                          throw new InvalidOperationException($"project {projectName} does not exists");

            var mergeRequests = await _client.GetMergeRequest(project.Id).All();
            var mergeRequest = mergeRequests.FirstOrDefault(s => s.Id == mergeRequestId);
            mergeRequest.Should().NotBeNull($"Merge request {mergeRequestId} does not exists");
            mergeRequest.Should().Match(predicate);
        }

        public static async Task DeleteAllMergeRequests(string projectName)
        {
            var projects = await _client.Projects.Accessible();

            var project = projects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName)) ??
                          throw new InvalidOperationException($"project {projectName} does not exists");

            var mergeRequests = await _client.GetMergeRequest(project.Id).All();
            await Task.WhenAll(mergeRequests.Select(
                m => _client.DeleteMergeRequest(project.Id, m.Id)));
        }
    }
}
