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
        private readonly Lazy<GitLabClientEx> _client;

        public GitLabFacade(Lazy<GitLabClientEx> client) => _client = client;

        public async Task<Result<int>> AddIssue(
            string title,
            string description,
            string projectName,
            string assigneeName = null,
            IEnumerable<string> labels = null)
        {
            return await SafeGetResult(async () =>
            {
                if (assigneeName.IsEmpty())
                    return await InnerAddIssue(title, description, projectName, null, labels);

                User assignee = await _client.Value.GetUserByNameAsync(assigneeName);
                return await InnerAddIssue(title, description, projectName, assignee?.Id, labels);
            });
        }

        public async Task<Result<int>> AddIssueForCurrentUser(
            string title,
            string description,
            string projectName,
            IEnumerable<string> labels = null)
        {
            return await SafeGetResult(
                async () => await InnerAddIssue(title, description, projectName, _client.Value.Users.Current.Id, labels));
        }

        public async Task<Result<int>> CreateMergeRequest(
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch)
        {
            return await SafeGetResult(
                () => InnerCreateMergeRequest(projectName, title, sourceBranch, targetBranch));
        }

        private async Task<Result<int>> InnerCreateMergeRequest(
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch)
        {
            var allProjects = await _client.Value.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<int>($"Project {projectName} was not found");

            IMergeRequestClient mergeRequestClient = _client.Value.GetMergeRequest(project.Id);

            var createdMergeRequest = await mergeRequestClient.CreateAsync(new MergeRequestCreate
            {
                SourceBranch = sourceBranch,
                TargetBranch = targetBranch,
                Title = title
            });

            return Result.Ok(createdMergeRequest.Id);
        }

        private async Task<Result<int>> InnerAddIssue(
            string title,
            string description,
            string projectName,
            int? assigneeId,
            IEnumerable<string> labels)
        {
            var allProjects = await _client.Value.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<int>($"Project {projectName} was not found");

            var createdIssue = await _client.Value.Issues.CreateAsync(new IssueCreate
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