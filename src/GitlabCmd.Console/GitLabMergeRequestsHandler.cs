using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitlabCmd.Console.GitLab;
using GitlabCmd.Console.Output;
using GitlabCmd.Console.Utilities;
using NGitLab.Models;

namespace GitlabCmd.Console
{
    public class GitLabMergeRequestsHandler
    {
        private readonly OutputPresenter _presenter;
        private readonly GitLabFacade _gitLabFacade;

        public GitLabMergeRequestsHandler(GitLabFacade gitLabFacade, OutputPresenter presenter)
        {
            _gitLabFacade = gitLabFacade;
            _presenter = presenter;
        }

        public async Task CreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            var mergeRequestResult = await InnerCreateMergeRequest(parameters);
            if (mergeRequestResult.IsFailure)
            {
                _presenter.FailureResult("Failed to create merge request", mergeRequestResult.Error);
                return;
            }

            _presenter.SuccessResult($"Successfully created merge request #{mergeRequestResult.Value}");
        }

        public async Task ListMerges(ListMergesParameters parameters)
        {
            var mergesResult = await InnerListMerges(parameters);
            if (mergesResult.IsFailure)
            {
                _presenter.FailureResult("Failed to retrieve merge requests", mergesResult.Error);
                return;
            }

            var merges = mergesResult.Value;
            if (merges.Count == 0)
            {
                _presenter.SuccessResult($"No merge requests found in project {parameters.Project}");
                return;
            }

            _presenter.GridResult(
                $"Found ({merges.Count}) merge requests in project {parameters.Project}",
                new[] { "Issue Id", "Title", "Assignee" },
                merges.Select(s => new object[] { s.Id, s.Title, s.Assignee }));
        }

        private async Task<Result<int>> InnerCreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            if (parameters.AssignedToCurrentUser)
            {
                return await _gitLabFacade.CreateMergeRequestForCurrentUser(
                    parameters.ProjectName,
                    parameters.Title,
                    parameters.SourceBranch,
                    parameters.TargetBranch);
            }

            return await _gitLabFacade.CreateMergeRequest(
                parameters.ProjectName,
                parameters.Title,
                parameters.SourceBranch,
                parameters.TargetBranch,
                parameters.Assignee);
        }

        private async Task<Result<IReadOnlyList<MergeRequest>>> InnerListMerges(ListMergesParameters parameters)
        {
            if (parameters.AssignedToCurrentUser)
            {
                return await _gitLabFacade.ListMergeRequestsForCurrentUser(
                    parameters.Project,
                    parameters.State);
            }

            return await _gitLabFacade.ListMergeRequests(
                parameters.Project,
                parameters.State,
                parameters.Assignee);
        }
    }
}
