using System.Threading.Tasks;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Utilities;

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
                _presenter.Info("-------------------------");
                _presenter.Info("Failed to create merge request");
                _presenter.Error($"{mergeRequestResult.Error}");
                return;
            }

            _presenter.Info("-------------------------");
            _presenter.Info($"Successfully created merge request #{mergeRequestResult.Value}");
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
    }
}
