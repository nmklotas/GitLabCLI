using System.Collections.Generic;
using System.Threading.Tasks;
using GitlabCmd.Core;
using GitlabCmd.Core.Gitlab;
using GitlabCmd.Core.Gitlab.Issues;
using GitlabCmd.Core.Gitlab.Merges;

namespace GitlabCmd.Gitlab
{
    public class GitLabFacade : IGitLabFacade
    {
        private readonly GitlabIssuesFacade _issuesFacade;
        private readonly GitlabMergesFacade _mergesFacade;
        private readonly Mapper _mapper;

        public GitLabFacade(
            GitlabIssuesFacade issuesFacade, 
            GitlabMergesFacade mergesFacade,
            Mapper mapper)
        {
            _issuesFacade = issuesFacade;
            _mergesFacade = mergesFacade;
            _mapper = mapper;
        }

        public async Task<Result<int>> CreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            if (parameters.AssignedToCurrentUser)
            {
                return await _mergesFacade.CreateMergeRequestForCurrentUser(
                    parameters.ProjectName,
                    parameters.Title,
                    parameters.SourceBranch,
                    parameters.TargetBranch);
            }

            return await _mergesFacade.CreateMergeRequest(
                parameters.ProjectName,
                parameters.Title,
                parameters.SourceBranch,
                parameters.TargetBranch,
                parameters.Assignee);
        }

        public async Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(ListMergesParameters parameters)
        {
            if (parameters.AssignedToCurrentUser)
            {
                return _mapper.Map(await _mergesFacade.ListMergeRequestsForCurrentUser(
                    parameters.Project,
                    parameters.State));
            }

            return _mapper.Map(await _mergesFacade.ListMergeRequests(
                parameters.Project,
                parameters.State,
                parameters.Assignee));
        }

        public async Task<Result<int>> CreateIssue(CreateIssueParameters parameters)
        {
            if (parameters.AssignToCurrentUser)
            {
                return await _issuesFacade.CreateIssueForCurrentUser(
                    parameters.Title,
                    parameters.Description,
                    parameters.ProjectName,
                    parameters.Labels);
            }

            return await _issuesFacade.CreateIssue(
                parameters.Title,
                parameters.Description,
                parameters.ProjectName,
                parameters.AssigneeName,
                parameters.Labels);
        }

        public async Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters)
        {
            if (parameters.AssignedToCurrentUser)
            {
                return _mapper.Map(await _issuesFacade.ListIssuesForCurrentUser(
                    parameters.Project,
                    parameters.Labels));
            }

            return _mapper.Map(await _issuesFacade.ListIssues(
                parameters.Project,
                parameters.Assignee,
                parameters.Labels));
        }
    }
}