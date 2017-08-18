using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitlabCmd.Console.Utilities;
using NGitLab;
using NGitLab.Impl;
using NGitLab.Models;

namespace GitlabCmd.Console.GitLab
{
    public class GitLabFacade
    {
        private readonly GitLabClientFactory _clientFactory;

        public GitLabFacade(GitLabClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<Result<int>> AddIssue(
            string title,
            string description,
            string projectName,
            string assigneeName = null,
            IEnumerable<string> labels = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();

            if (assigneeName.IsEmpty())
                return await InnerAddIssue(client, title, description, projectName, null, labels);

            User assignee = await client.GetUserByNameAsync(assigneeName);
            return await InnerAddIssue(client, title, description, projectName, assignee?.Id, labels);
        });

        public async Task<Result<int>> AddIssueForCurrentUser(
            string title,
            string description,
            string projectName,
            IEnumerable<string> labels = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();
            return await InnerAddIssue(client, title, description, projectName, client.Users.Current.Id, labels);
        });

        public async Task<Result<int>> CreateMergeRequest(
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();
            return await InnerCreateMergeRequest(client, projectName, title, sourceBranch, targetBranch);
        });

        private async Task<Result<int>> InnerCreateMergeRequest(
            GitLabClientEx client,
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch)
        {
            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<int>($"Project {projectName} was not found");

            IMergeRequestClient mergeRequestClient = client.GetMergeRequest(project.Id);

            var createdMergeRequest = await mergeRequestClient.CreateAsync(new MergeRequestCreate
            {
                SourceBranch = sourceBranch,
                TargetBranch = targetBranch,
                Title = title
            });

            return Result.Ok(createdMergeRequest.Id);
        }

        private async Task<Result<int>> InnerAddIssue(
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