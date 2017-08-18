using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.Utilities;
using NGitLab;
using NGitLab.Models;

namespace GitlabCmd.Console.Test.GitLab
{
    public static class GitLabApiHelper
    {
        private static GitLabClient _client;

        public static string ProjectName => "testproject";

        public static string UserName => "nmklotas";

        public static string NonExistingProjectName => Guid.NewGuid().ToString();

        public static GitLabClient Client =>
            _client ?? (_client = new GitLabClient("https://gitlab.com/api/v3", "KZKSRcxxHi82r4D4p_aJ"));

        public static async Task ShouldHaveIssue(
            string projectName, 
            int issueId,
            Action<Issue> issueAction)
        {
            var projects = await Client.Projects.Accessible();

            var project = projects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName)) ??
                throw new InvalidOperationException($"project {projectName} does not exists");

            var issue = await Client.Issues.GetAsync(project.Id, issueId);
            issue.Should().NotBeNull($"Issue {issueId} does not exists");
            issueAction(issue);
        }
    }
}
