using System.Collections.Generic;
using System.Linq;
using GitLabCLI.Console.Configuration;
using GitLabCLI.Console.Parameters;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Core.Gitlab.Merges;
using GitLabCLI.Utilities;
using Result = GitLabCLI.Core.Result;

namespace GitLabCLI.Console
{
    public sealed class ParametersHandler
    {
        private readonly AppSettings _appSettings;
        private readonly IssueParametersNegotiator _issueParametersNegotiator;
        private readonly MergeRequestsParametersNegotiator _mergesParametersNegotiator;
        private readonly ConfigurationParametersNegotiator _configurationParametersNegotiator;

        public ParametersHandler(
            AppSettings appSettings, 
            IssueParametersNegotiator issueParametersNegotiator, 
            MergeRequestsParametersNegotiator mergesParametersNegotiator, 
            ConfigurationParametersNegotiator configurationParametersNegotiator)
        {
            _appSettings = appSettings;
            _issueParametersNegotiator = issueParametersNegotiator;
            _mergesParametersNegotiator = mergesParametersNegotiator;
            _configurationParametersNegotiator = configurationParametersNegotiator;
        }

        public Result<CreateIssueParameters> NegotiateCreateIssueParameters(CreateIssueOptions options)
        {
            return _issueParametersNegotiator.NegotiateCreateIssueParameters(
                options, _appSettings.DefaultProject,
                _appSettings.DefaulIssuesLabel);
        }

        public ConfigurationParameters NegotiateConfigurationParameters(ConfigurationOptions options)
        {
            return _configurationParametersNegotiator.NegotiateConfigurationParameters(options);
        }

        public Result<ListMergesParameters> NegotiateListMergesParameters(ListMergesOptions options)
        {
            return _mergesParametersNegotiator.NegotiateListMergeRequestsParameters(options, _appSettings.DefaultProject);
        }

        public Result<ListIssuesParameters> NegotiateListIssuesParameters(ListIssuesOptions options)
        {
            return _issueParametersNegotiator.NegotiateListIssuesParameters(options, _appSettings.DefaultProject, _appSettings.DefaulIssuesLabel);
        }

        public Result<CreateMergeRequestParameters> NegotiateCreateMergeRequestParameters(CreateMergeRequestOptions options)
        {
            return _mergesParametersNegotiator.NegotiateCreateMergeRequestParameters(options, _appSettings.DefaultProject);
        }

        public Result<BrowseParameters> NegotiateBrowseParameters(BrowseOptions options)
        {
            return _issueParametersNegotiator.NegotiateBrowseParameters(options, _appSettings.DefaultProject);
        }

        public Result<CloseIssueParameters> NegotiateCloseIssueParameters(CloseIssueOptions options)
        {
            return _issueParametersNegotiator.NegotiateCloseIssueParameters(options, _appSettings.DefaultProject);
        }
    }
}
