using System.Collections.Generic;
using System.Threading.Tasks;
using GitlabCmd.Console.App;
using GitlabCmd.Console.Utilities;
using NGitLab.Models;

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

        public async Task AddIssue(CreateIssueParameters parameters)
        {
            var issueResult = await InnerAddIssue(parameters);
            if (issueResult.IsFailure)
            {
                _presenter.Info("Failed to create issue");
                _presenter.Error($"{issueResult.Error}");
                return;
            }

            _presenter.Info($"Successfully created issue #{issueResult.Value}");
        }

        public async Task ListIssues(ListIssuesParameters parameters)
        {
            var issueResult = await InnerListIssues(parameters);
            if (issueResult.IsFailure)
            {
                _presenter.Info("Failed to retrieve issues");
                _presenter.Error($"{issueResult.Error}");
                return;
            }

            foreach (var issue in issueResult.Value)
                _presenter.Info($"#{issue.Id} - {issue.Title} - {issue.Description}");
        }

        private async Task<Result<IReadOnlyList<Issue>>> InnerListIssues(ListIssuesParameters parameters)
        {
            if (parameters.AssignedToCurrentUser)
            {
                return await _gitLabFacade.ListIssuesForCurrentUser(
                    parameters.ProjectName, 
                    parameters.Labels);
            }

            return await _gitLabFacade.ListIssues(
                parameters.ProjectName,
                parameters.Assignee,
                parameters.Labels);
        }

        private async Task<Result<int>> InnerAddIssue(CreateIssueParameters parameters)
        {
            if (parameters.AssignToCurrentUser)
            {
                return await _gitLabFacade.AddIssueForCurrentUser(
                    parameters.Title,
                    parameters.Description,
                    parameters.ProjectName,
                    parameters.Labels);
            }

            return await _gitLabFacade.AddIssue(
                parameters.Title,
                parameters.Description,
                parameters.ProjectName,
                parameters.AssigneeName,
                parameters.Labels);
        }
    }
}
