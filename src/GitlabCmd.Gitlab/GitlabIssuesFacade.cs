using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Issues;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Utilities;
using Issue = GitLabApiClient.Models.Issues.Issue;

namespace GitLabCLI.GitLab
{
    public sealed class GitLabIssuesFacade
    {
        private readonly GitLabClientFactory _clientFactory;

        public GitLabIssuesFacade(GitLabClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<Result<int>> CreateIssue(CreateIssueParameters parameters)
        {
            var client = await _clientFactory.Create();

            var project = (await client.Projects.GetAsync(parameters.Project)).FirstOrDefault();
            if (project == null)
                return Result.Fail<int>($"Project {parameters.Project} was not found");

            int? assigneeId = await GetUserId(client, parameters.AssignedToCurrentUser, parameters.Assignee);
            
            var createdIssue = await client.Issues.CreateAsync(new CreateIssueRequest(project.Id, parameters.Title)
            {
                Description = parameters.Description,
                Labels = parameters.Labels.Any() ? string.Join(",", parameters.Labels) : null,
                Assignees = assigneeId.HasValue ? new List<int> {  assigneeId.Value} : null, 
            });

            return Result.Ok(createdIssue.Iid);
        }

        public async Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters)
        {
            var client = await _clientFactory.Create();

            var project = (await client.Projects.GetAsync(parameters.Project)).FirstOrDefault();
            if (project == null)
                return Result.Fail<IReadOnlyList<Issue>>($"Project {parameters.Project} was not found");

            int? assigneeId = await GetUserId(client, parameters.AssignedToCurrentUser, parameters.Assignee);

            IEnumerable<Issue> issues = await client.Issues.GetAsync(project.Id);
            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId);

            if (parameters.Labels.Any())
                issues = issues.Where(i => i.Labels != null && i.Labels.Contains(parameters.Labels));

            return Result.Ok<IReadOnlyList<Issue>>(issues.ToList());
        }

        private static async Task<int?> GetUserId(GitLabClient client, bool isCurrentUser, string name)
        {
            if (isCurrentUser)
                return (await client.Users.GetCurrentSessionAsync()).Id;

            if (name.IsNullOrEmpty())
                return null;

            return (await client.Users.GetAsync(name)).Id;
        }
    }
}
