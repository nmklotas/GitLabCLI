using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Issues.Requests;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Utilities;
using Issue = GitLabApiClient.Models.Issues.Responses.Issue;

namespace GitLabCLI.GitLab
{
    public sealed class GitLabIssuesFacade
    {
        private readonly GitLabClientFactory _clientFactory;

        public GitLabIssuesFacade(GitLabClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<Result<int>> CreateIssue(CreateIssueParameters parameters)
        {
            var client = await _clientFactory.Create();

            string projectId = await GetProjectIdAsync(client, parameters.Project);
            if (projectId == null)
                return Result.Fail<int>($"Project {parameters.Project} was not found");

            int? assigneeId = await GetUserId(client, parameters.AssignedToCurrentUser, parameters.Assignee);

            var createdIssue = await client.Issues.CreateAsync(new CreateIssueRequest(projectId, parameters.Title)
            {
                Description = parameters.Description,
                Labels = parameters.Labels,
                Assignees = assigneeId.HasValue ? new List<int> { assigneeId.Value } : null,
            });

            return Result.Ok(createdIssue.Iid);
        }

        public async Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters)
        {
            var client = await _clientFactory.Create();

            string projectId = await GetProjectIdAsync(client, parameters.Project);
            if (projectId == null)
                return Result.Fail<IReadOnlyList<Issue>>($"Project {parameters.Project} was not found");

            int? assigneeId = await GetUserId(client, parameters.AssignedToCurrentUser, parameters.Assignee);

            var issues = (await client.Issues.GetAsync(projectId)).ToList();
            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId).ToList();

            if (parameters.Labels.Any())
                issues = issues.Where(i => i.Labels != null && i.Labels.Contains(parameters.Labels)).ToList();

            return Result.Ok<IReadOnlyList<Issue>>(issues);
        }

        private static async Task<int?> GetUserId(GitLabClient client, bool isCurrentUser, string name)
        {
            if (isCurrentUser)
                return (await client.Users.GetCurrentSessionAsync()).Id;

            if (name.IsNullOrEmpty())
                return null;

            return (await client.Users.GetAsync(name)).Id;
        }

        private async Task<string> GetProjectIdAsync(GitLabClient client, string project)
        {
            var gitLabProject = (await client.Projects.GetAsync(o =>
                {
                    o.Filter = project;
                    o.Simple = true;
                })).
                FirstOrDefault();

            if (gitLabProject == null)
                return null;

            return gitLabProject.Id.ToString();
        }
    }
}
