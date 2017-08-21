using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using GitlabCmd.Console.Cmd;
using GitlabCmd.Console.GitLab;

namespace GitlabCmd.Console.App
{
    public class LaunchHandler
    {
        private readonly Parser _parser;
        private readonly ParametersHandler _parametersHandler;
        private readonly GitLabIssueHandler _issueHandler;
        private readonly ConfigurationHandler _configurationHandler;
        private readonly GitLabMergeRequestsHandler _mergeRequestHandler;

        public LaunchHandler(
            Parser parser, 
            ParametersHandler parametersHandler,
            ConfigurationHandler configurationHandler,
            GitLabMergeRequestsHandler mergeRequestHandler,
            GitLabIssueHandler issueHandler)
        {
            _parser = parser;
            _parametersHandler = parametersHandler;
            _configurationHandler = configurationHandler;
            _mergeRequestHandler = mergeRequestHandler;
            _issueHandler = issueHandler;
        }

        public async Task<int> Launch(string[] args) => await _parser.
            ParseVerbs<
                IssueOptions,
                MergeOptions,
                ConfigurationOptions,
                Task<int>>(args).
            MapResult(
                (IssueOptions o) => Handle(o),
                (MergeOptions o) => Handle(o),
                (ConfigurationOptions o) => Handle(o),
                Handle);

        private async Task<int> Handle(IssueOptions options)
        {
            if (!_configurationHandler.IsConfigurationValid())
                return ExitCode.InvalidConfiguration;

            if (options is CreateIssueOptions createIssueOptions)
            {
                var parameters = _parametersHandler.NegotiateAddIssueParameters(createIssueOptions);
                if (parameters.IsSuccess)
                {
                    await _issueHandler.AddIssue(parameters.Value);
                }
            }

            if (options is ListIssuesOptions listIssueOptions)
            {
                var parameters = _parametersHandler.NegotiateListIssuesParameters(listIssueOptions);
                if (parameters.IsSuccess)
                {
                    await _issueHandler.ListIssues(parameters.Value);
                }
            }

            return ExitCode.Success;
        }

        private async Task<int> Handle(MergeOptions options)
        {
            if (!_configurationHandler.IsConfigurationValid())
                return ExitCode.InvalidConfiguration;

            if (options is CreateMergeRequestOptions createMergeRequestOptions)
            {
                var parameters = _parametersHandler.NegotiateCreateMergeRequestParameters(createMergeRequestOptions);
                if (parameters.IsSuccess)
                {
                    await _mergeRequestHandler.CreateMergeRequest(parameters.Value);
                }
            }

            if (options is ListMergesOptions listMergesOptions)
            {
                var parameters = _parametersHandler.NegotiateListMergesParameters(listMergesOptions);
                if (parameters.IsSuccess)
                {
                    await _mergeRequestHandler.ListMerges(parameters.Value);
                }
            }

            return ExitCode.Success;
        }

        private Task<int> Handle(ConfigurationOptions options)
        {
            var parameters = _parametersHandler.NegotiateConfigurationParameters(options);
            _configurationHandler.StoreConfiguration(parameters);
            return Task.FromResult(ExitCode.Success);
        }

        private Task<int> Handle(IEnumerable<Error> errors) =>
            Task.FromResult(ExitCode.InvalidArguments);
    }
}
