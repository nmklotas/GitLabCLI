using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GitLabCLI.Console.Output;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab;
using GitLabCLI.Core.Gitlab.Issues;

namespace GitLabCLI.Console
{
    public sealed class IssueBrowseHandler
    {
        private readonly IGitLabFacade _issuesFacade;
        private readonly IBrowser _browser;
        private readonly OutputPresenter _outputPresenter;

        public IssueBrowseHandler(IGitLabFacade issuesFacade, IBrowser browser, OutputPresenter outputPresenter)
        {
            _issuesFacade = issuesFacade;
            _browser = browser;
            _outputPresenter = outputPresenter;
        }

        public async Task Browse(BrowseParameters parameters)
        {
            var issuesResult = await _issuesFacade.ListIssues(new ListIssuesParameters(parameters.Project)
            {
                IssuesIds = new[] { parameters.IssueId }
            });

            if (issuesResult.IsFailure)
            {
                _outputPresenter.ShowError("Failed to browse to issue", issuesResult.Error);
                return;
            }

            var firstIssue = issuesResult.Value.FirstOrDefault();
            if (firstIssue == null)
            {
                _outputPresenter.ShowMessage($"Issue #{parameters.IssueId} was not found in project {parameters.Project}");
                return;
            }

            try
            {
                _browser.Open(firstIssue.WebUrl);
            }
            catch (Win32Exception ex)
            {
                _outputPresenter.ShowError($"Failed to open {firstIssue.WebUrl}: {ex}");
            }
        }
    }
}
