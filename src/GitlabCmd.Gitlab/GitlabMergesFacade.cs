using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Merges;
using GitLabCLI.Utilities;
using NGitLab.Models;
using MergeRequest = NGitLab.Models.MergeRequest;

namespace GitLabCLI.GitLab
{
    public sealed class GitLabMergesFacade
    {
        private readonly GitLabClientExFactory _clientFactory;
        private readonly Mapper _mapper;

        public GitLabMergesFacade(GitLabClientExFactory clientFactory, Mapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        public async Task<Result<int>> CreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            var client = await _clientFactory.Create();

            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(parameters.Project));
            if (project == null)
                return Result.Fail<int>($"Project {parameters.Project} was not found");

            int? assigneeId = await client.GetUserId(parameters.AssignedToCurrentUser, parameters.Assignee);

            var createdMergeRequest = await client.CreateMergeAsync(project.Id, new MergeRequestCreate
            {
                SourceBranch = parameters.SourceBranch,
                TargetBranch = parameters.TargetBranch,
                Title = parameters.Title,
                AssigneeId = assigneeId,
                TargetProjectId = project.Id
            });

            return Result.Ok(createdMergeRequest.Id);
        }

        public async Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(ListMergesParameters parameters)
        {
            var client = await _clientFactory.Create();

            var allProjects = await client.Projects.Accessible();
            var project = allProjects.FirstOrDefault(p => p.Name.EqualsIgnoringCase(parameters.Project));
            if (project == null)
                return Result.Fail<IReadOnlyList<MergeRequest>>($"Project {parameters.Project} was not found");

            var issues = parameters.State.HasValue ?
                await client.GetMergeRequest(project.Id).AllInState(_mapper.Map(parameters.State.Value)) :
                await client.GetMergeRequest(project.Id).All();

            int? assigneeId = await client.GetUserId(parameters.AssignedToCurrentUser, parameters.Assignee);

            if (assigneeId.HasValue)
                issues = issues.Where(i => i.Assignee?.Id == assigneeId);

            return Result.Ok<IReadOnlyList<MergeRequest>>(issues.ToList());
        }
    }
}
