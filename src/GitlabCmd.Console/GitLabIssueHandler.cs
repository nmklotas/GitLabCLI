using System.Linq;
using System.Threading.Tasks;
using GitLabCmd.Console.Output;
using GitLabCmd.Core.GitLab;
using GitLabCmd.Core.GitLab.Issues;

namespace GitLabCmd.Console
{
    public sealed class GitLabIssueHandler
    {
        private readonly IGitLabFacade _gitLabFacade;
        private readonly OutputPresenter _presenter;

        public GitLabIssueHandler(
            IGitLabFacade gitLabFacade,
            OutputPresenter presenter)
        {
            _gitLabFacade = gitLabFacade;
            _presenter = presenter;
        }

        public async Task CreateIssue(CreateIssueParameters parameters)
        {
            var issueResult = await _gitLabFacade.CreateIssue(parameters);
            if (issueResult.IsFailure)
            {
                _presenter.FailureResult("Failed to create issue", issueResult.Error);
                return;
            }

            _presenter.SuccessResult($"Successfully created issue #{issueResult.Value}");
        }

        public async Task ListIssues(ListIssuesParameters parameters)
        {
            var issueResult = await _gitLabFacade.ListIssues(parameters);
            if (issueResult.IsFailure)
            {
                _presenter.FailureResult("Failed to retrieve issues", issueResult.Error);
                return;
            }

            var issues = issueResult.Value;
            if (issues.Count == 0)
            {
                _presenter.SuccessResult($"No issues found in project {parameters.Project}");
                return;
            }

            _presenter.GridResult(
                $"Found ({issues.Count}) issues in project {parameters.Project}", 
                new[] { "Issue Id", "Title", "Description"},
                issues.Select(s => new object[] { s.Id, s.Title, s.Description }));
        }
    }
}
