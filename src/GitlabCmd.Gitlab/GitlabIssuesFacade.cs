using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitlabCmd.Core;
using GitlabCmd.Utilities;
using NGitLab.Impl;
using NGitLab.Models;

namespace GitlabCmd.Gitlab
{
    public sealed class GitlabIssuesFacade
    {
        private readonly GitLabClientExFactory _clientFactory;

        public GitlabIssuesFacade(GitLabClientExFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<Result<int>> CreateIssue(
            string title,
            string description,
            string projectName,
            string assigneeName = null,
            IEnumerable<string> labels = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();

            if (assigneeName.IsNullOrEmpty())
                return await InnerCreateIssue(client, title, description, projectName, null, labels);

            User assignee = await client.GetUserByNameAsync(assigneeName);
            return await InnerCreateIssue(client, title, description, projectName, assignee?.Id, labels);
        });

        public async Task<Result<int>> CreateIssueForCurrentUser(
            string title,
            string description,
            string projectName,
            IEnumerable<string> labels = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();
            return await InnerCreateIssue(client, title, description, projectName, client.Users.Current.Id, labels);
        });

        public async Task<Result<IReadOnlyList<Issue>>> ListIssues(
            string projectName,
            string assigneeName = null,
            IEnumerable<string> labels = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();

            if (assigneeName.IsNullOrEmpty())
                return await InnerListIssues(client, projectName, null, labels);

            User assignee = await client.GetUserByNameAsync(assigneeName);
            return await InnerListIssues(client, projectName, assignee?.Id, labels);
        });

        public async Task<Result<IReadOnlyList<Issue>>> ListIssuesForCurrentUser(
            string projectName,
            IEnumerable<string> labels = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();
            return await InnerListIssues(client, projectName, client.Users.Current.Id, labels);
        });

        private async Task<Result<int>> InnerCreateIssue(
            GitLabClientEx client,
            string title,
            string description,
            string projectName,
            int? assigneeId,
            IEnumerable<string> labels)
        {
            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<int>($"Project {projectName} was not found");

            var createdIssue = await client.Issues.CreateAsync(new IssueCreate
            {
                ProjectId = project.Id,
                Title = title,
                Description = description,
                Labels = labels != null ? string.Join(",", labels) : null,
                AssigneeId = assigneeId
            });

            return Result.Ok(createdIssue.Id);
        }

        private async Task<Result<IReadOnlyList<Issue>>> InnerListIssues(
            GitLabClientEx client,
            string projectName,
            int? assigneeId,
            IEnumerable<string> labels = null)
        {
            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<IReadOnlyList<Issue>>($"Project {projectName} was not found");

            var issues = await client.Issues.ForProject(project.Id);
            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId);

            var inputLabels = labels.SafeEnumerate();
            if (inputLabels.Any())
                issues = issues.Where(i => i.Labels != null && i.Labels.Contains(inputLabels));

            return Result.Ok<IReadOnlyList<Issue>>(issues.ToList());
        }

        private static async Task<Result<T>> SafeGetResult<T>(Func<Task<Result<T>>> resultDelegate)
        {
            try
            {
                return await resultDelegate();
            }
            catch (OperationCanceledException)
            {
                return Result.Fail<T>("Request timed out");
            }
            catch (GitLabException ex)
            {
                return Result.Fail<T>($"Request failed. {ex.Message}");
            }
        }
    }
}
