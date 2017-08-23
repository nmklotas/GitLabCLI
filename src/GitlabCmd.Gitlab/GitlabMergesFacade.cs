using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitlabCmd.Core;
using GitlabCmd.Utilities;
using NGitLab.Impl;
using NGitLab.Models;
using MergeRequestState = GitlabCmd.Core.Gitlab.Merges.MergeRequestState;

namespace GitlabCmd.Gitlab
{
    public sealed class GitlabMergesFacade
    {
        private readonly GitLabClientExFactory _clientFactory;
        private readonly Mapper _mapper;

        public GitlabMergesFacade(GitLabClientExFactory clientFactory, Mapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        public async Task<Result<int>> CreateMergeRequest(
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch,
            string assigneeName = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();

            if (assigneeName.IsNullOrEmpty())
                return await InnerCreateMergeRequest(client, projectName, title, sourceBranch, targetBranch, null);

            User assignee = await client.GetUserByNameAsync(assigneeName);
            return await InnerCreateMergeRequest(client, projectName, title, sourceBranch, targetBranch, assignee?.Id);
        });

        public async Task<Result<int>> CreateMergeRequestForCurrentUser(
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();
            return await InnerCreateMergeRequest(client, projectName, title, sourceBranch, targetBranch, client.Users.Current.Id);
        });

        public async Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(
            string projectName,
            MergeRequestState? state,
            string assigneeName = null) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();

            if (assigneeName.IsNullOrEmpty())
                return await InnerListMerges(client, projectName, null, state);

            User assignee = await client.GetUserByNameAsync(assigneeName);
            return await InnerListMerges(client, projectName, assignee?.Id, state);
        });

        public async Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequestsForCurrentUser(
            string projectName,
            MergeRequestState? state) => await SafeGetResult(async () =>
        {
            var client = await _clientFactory.Create();
            return await InnerListMerges(client, projectName, client.Users.Current.Id, state);
        });

        private async Task<Result<int>> InnerCreateMergeRequest(
            GitLabClientEx client,
            string projectName,
            string title,
            string sourceBranch,
            string targetBranch,
            int? assigneeId)
        {
            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<int>($"Project {projectName} was not found");

            var createdMergeRequest = await client.CreateMergeAsync(project.Id, new MergeRequestCreate
            {
                SourceBranch = sourceBranch,
                TargetBranch = targetBranch,
                Title = title,
                AssigneeId = assigneeId,
                TargetProjectId = project.Id
            });

            return Result.Ok(createdMergeRequest.Id);
        }

        private async Task<Result<IReadOnlyList<MergeRequest>>> InnerListMerges(
            GitLabClientEx client,
            string projectName,
            int? assigneeId,
            MergeRequestState? state)
        {
            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(projectName));
            if (project == null)
                return Result.Fail<IReadOnlyList<MergeRequest>>($"Project {projectName} was not found");

            var issues = state.HasValue ?
                await client.GetMergeRequest(project.Id).AllInState(_mapper.Map(state.Value)) :
                await client.GetMergeRequest(project.Id).All();

            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId);

            return Result.Ok<IReadOnlyList<MergeRequest>>(issues.ToList());
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
