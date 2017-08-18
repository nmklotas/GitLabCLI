using System.Threading.Tasks;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Utilities;

namespace GitlabCmd.Console.GitLab
{
    public class GitLabIssueHandler
    {
        private readonly GitLabFacade _gitLabFacade;
        private readonly OutputPresenter _presenter;

        public GitLabIssueHandler(
            GitLabFacade gitLabFacade,
            OutputPresenter presenter)
        {
            _gitLabFacade = gitLabFacade;
            _presenter = presenter;
        }

        public async Task AddIssue(AddIssueParameters parameters)
        {
            Result<int> issueResult = await _gitLabFacade.AddIssue(
                parameters.Title,
                parameters.Description,
                parameters.ProjectName,
                parameters.Labels);

            if (issueResult.IsFailure)
            {
                _presenter.Info("Failed to create issue");
                _presenter.Info($"Error: {issueResult.Error}");
                return;
            }

            _presenter.Info($"Successfully created issue #{issueResult.Value}");
        }
    }
}
