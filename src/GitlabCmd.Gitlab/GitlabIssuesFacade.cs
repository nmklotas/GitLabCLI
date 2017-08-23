using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitlabCmd.Core;
using GitlabCmd.Core.Gitlab.Issues;
using GitlabCmd.Utilities;
using NGitLab.Models;
using Issue = NGitLab.Models.Issue;

namespace GitlabCmd.Gitlab
{
    public sealed class GitlabIssuesFacade
    {
        private readonly GitLabClientExFactory _clientFactory;

        public GitlabIssuesFacade(GitLabClientExFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<Result<int>> CreateIssue(CreateIssueParameters parameters)
        {
            var client = await _clientFactory.Create();

            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(parameters.Project));
            if (project == null)
                return Result.Fail<int>($"Project {parameters.Project} was not found");

            int? assigneeId = await client.GetUserId(parameters.AssignedToCurrentUser, parameters.Assignee);

            var createdIssue = await client.Issues.CreateAsync(new IssueCreate
            {
                ProjectId = project.Id,
                Title = parameters.Title,
                Description = parameters.Description,
                Labels = parameters.Labels.Any() ? string.Join(",", parameters.Labels) : null,
                AssigneeId = assigneeId
            });

            return Result.Ok(createdIssue.Id);
        }

        public async Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters)
        {
            var client = await _clientFactory.Create();

            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(parameters.Project));
            if (project == null)
                return Result.Fail<IReadOnlyList<Issue>>($"Project {parameters.Project} was not found");

            int? assigneeId = await client.GetUserId(parameters.AssignedToCurrentUser, parameters.Assignee);

            var issues = await client.Issues.ForProject(project.Id);
            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId);

            if (parameters.Labels.Any())
                issues = issues.Where(i => i.Labels != null && i.Labels.Contains(parameters.Labels));

            return Result.Ok<IReadOnlyList<Issue>>(issues.ToList());
        }
    }
}
