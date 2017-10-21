using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GitLabCLI.Console.Output;
using GitLabCLI.Core.Gitlab;
using GitLabCLI.Core.Gitlab.Issues;

namespace GitLabCLI.Console
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

            if (parameters.Format == OutputFormat.Rows)
            {
                _presenter.RowResult(
                    $"Found ({issues.Count}) issues in project {parameters.Project}",
                    issues.Select(i => new Row(new[]
                    {
                        $"{ConsoleOutputColor.Yellow}#{i.Id}",
                        "Author",
                        "Created"
                    }, 
                    new[]
                    {
                        i.Title,
                        i.Author,
                        i.CreatedAt.ToString(CultureInfo.CurrentUICulture),
                    }, 
                    i.Description)).
                    ToArray());
            }
            else if (parameters.Format == OutputFormat.Grid)
            {
                _presenter.GridResult(
                    $"Found ({issues.Count}) issues in project {parameters.Project}",
                    new GridColumn<int>("Issue Id", 20, issues.Select(s => s.Id)),
                    new GridColumn("Title", 150, issues.Select(s => s.Title)),
                    new GridColumn("Description", 50, issues.Select(s => s.Description)));
            }
            else
            {
                throw new NotSupportedException($"OutputFormat {parameters.Format} is not supported");
            }
        }
    }
}
