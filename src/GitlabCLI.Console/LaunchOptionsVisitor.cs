using System.Threading.Tasks;
using GitLabCLI.Console.Configuration;
using GitLabCLI.Console.Output;
using GitLabCLI.Console.Parameters;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core;

namespace GitLabCLI.Console
{
    public sealed class LaunchOptionsVisitor
    {
        private readonly GitLabIssueHandler _issuesHandler;
        private readonly GitLabMergeRequestsHandler _mergesHandler;
        private readonly ConfigurationHandler _configurationHandler;
        private readonly ParametersHandler _parametersHandler;
        private readonly OutputPresenter _outputPresenter;
        private readonly IssueBrowseHandler _issueBrowseHandler;

        public LaunchOptionsVisitor(
            GitLabIssueHandler issuesHandler,
            GitLabMergeRequestsHandler mergesHandler,
            ConfigurationHandler configurationHandler,
            ParametersHandler parametersHandler,
            IssueBrowseHandler issueBrowseHandler,
            OutputPresenter outputPresenter)
        {
            _issuesHandler = issuesHandler;
            _mergesHandler = mergesHandler;
            _configurationHandler = configurationHandler;
            _parametersHandler = parametersHandler;
            _issueBrowseHandler = issueBrowseHandler;
            _outputPresenter = outputPresenter;
        }

        public async Task Visit(CreateIssueOptions options)
        {
            if (!ValidateConfiguration())
                return;

            var parameters = _parametersHandler.NegotiateCreateIssueParameters(options);
            if (parameters.IsSuccess)
                await _issuesHandler.CreateIssue(parameters.Value);
            else
                ShowError(parameters);
        }

        public async Task Visit(CloseIssueOptions options)
        {
            if (!ValidateConfiguration())
                return;

            var parameters = _parametersHandler.NegotiateCloseIssueParameters(options);
            if (parameters.IsSuccess)
                await _issuesHandler.CloseIssue(parameters.Value);
            else
                ShowError(parameters);
        }

        public async Task Visit(BrowseOptions options)
        {
            if (!ValidateConfiguration())
                return;

            var parameters = _parametersHandler.NegotiateBrowseParameters(options);
            if (parameters.IsSuccess)
                await _issueBrowseHandler.Browse(parameters.Value);
            else
                ShowError(parameters);
        }

        public async Task Visit(CreateMergeRequestOptions options)
        {
            if (!ValidateConfiguration())
                return;

            var parameters = _parametersHandler.NegotiateCreateMergeRequestParameters(options);
            if (parameters.IsSuccess)
                await _mergesHandler.CreateMergeRequest(parameters.Value);
            else
                ShowError(parameters);
        }

        public async Task Visit(ListIssuesOptions options)
        {
            if (!ValidateConfiguration())
                return;

            var parameters = _parametersHandler.NegotiateListIssuesParameters(options);
            if (parameters.IsSuccess)
                await _issuesHandler.ListIssues(parameters.Value);
            else
                ShowError(parameters);
        }

        public async Task Visit(ListMergesOptions options)
        {
            if (!ValidateConfiguration())
                return;

            var parameters = _parametersHandler.NegotiateListMergesParameters(options);
            if (parameters.IsSuccess)
                await _mergesHandler.ListMerges(parameters.Value);
            else
                ShowError(parameters);
        }

        public Task Visit(ConfigurationOptions options)
        {
            var parameters = _parametersHandler.NegotiateConfigurationParameters(options);
            _configurationHandler.StoreConfiguration(parameters);
            return Task.CompletedTask;
        }

        private bool ValidateConfiguration()
        {
            var validationResult = _configurationHandler.Validate();
            if (validationResult.IsFailure)
            {
                _outputPresenter.ShowMessage(validationResult.Error);
                return false;
            }

            return true;
        }

        private void ShowError(Result result) 
            => _outputPresenter.ShowError(result.Error);
    }
}
