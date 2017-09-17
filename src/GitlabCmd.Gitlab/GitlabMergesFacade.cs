﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Merges;
using MergeRequest = GitLabApiClient.Models.MergeRequests.Responses.MergeRequest;

namespace GitLabCLI.GitLab
{
    public sealed class GitLabMergesFacade
    {
        private readonly GitLabClientFactory _clientFactory;
        private readonly Mapper _mapper;

        public GitLabMergesFacade(GitLabClientFactory clientFactory, Mapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        public async Task<Result<int>> CreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            var client = await _clientFactory.Create();

            int? projecId = await GetProjectId(client, parameters.Project);
            if (!projecId.HasValue)
                return Result.Fail<int>($"Project {parameters.Project} was not found");

            int? assigneeId = await GetUserId(client, parameters.AssignedToCurrentUser, parameters.Assignee);

            var createdMergeRequest = await client.MergeRequests.CreateAsync(
                new CreateMergeRequest(
                    projecId.Value.ToString(), 
                    parameters.SourceBranch, 
                    parameters.TargetBranch, 
                    parameters.Title)
            {
                AssigneeId = assigneeId
            });

            return Result.Ok(createdMergeRequest.Iid);
        }

        public async Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(ListMergesParameters parameters)
        {
            var client = await _clientFactory.Create();

            int? projectId = await GetProjectId(client, parameters.Project);
            if (!projectId.HasValue)
                return Result.Fail<IReadOnlyList<MergeRequest>>($"Project {parameters.Project} was not found");

            IEnumerable<MergeRequest> issues = parameters.State.HasValue ?
                await client.MergeRequests.GetAsync(projectId.Value, o => o.State = _mapper.Map(parameters.State.Value)) :
                await client.MergeRequests.GetAsync(projectId.Value);

            int? assigneeId = await GetUserId(client, parameters.AssignedToCurrentUser, parameters.Assignee);

            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId);

            return Result.Ok<IReadOnlyList<MergeRequest>>(issues.ToList());
        }

        private static async Task<int?> GetUserId(GitLabClient client, bool isCurrentUser, string name)
        {
            if (isCurrentUser)
                return (await client.Users.GetCurrentSessionAsync()).Id;

            if (string.IsNullOrEmpty(name))
                return null;

            return (await client.Users.GetAsync(name)).Id;
        }

        private static async Task<int?> GetProjectId(GitLabClient client, string project)
        {
            var gitLabProject = (await client.Projects.GetAsync(o =>
                {
                    o.Filter = project;
                    o.Simple = true;
                })).
                FirstOrDefault();

            if (gitLabProject == null)
                return null;

            return gitLabProject.Id;
        }
    }
}
