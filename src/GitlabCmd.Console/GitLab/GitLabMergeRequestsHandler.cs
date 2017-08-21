using System.Collections.Generic;
using System.Threading.Tasks;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Utilities;
using NGitLab.Models;

namespace GitlabCmd.Console.GitLab
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
            var issueResult = await InnerListMerges(parameters);
            if (issueResult.IsFailure)
            {
                _presenter.FailureResult("Failed to retrieve merge requests", issueResult.Error);
                return;
            }

            var issues = issueResult.Value;
            _presenter.Info("-------------------------");
            _presenter.Info($"Merge requests ({issues.Count})");
            foreach (var issue in issues)
            {
                _presenter.Info($"#{issue.Id} - {issue.Title} - {issue.Description}");
            }
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
