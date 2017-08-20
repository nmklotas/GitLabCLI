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
                CreateOptions,
                IssueOptions,
                ConfigurationOptions,
                Task<int>>(args).
            MapResult(
                (CreateOptions o) => Create(o),
                (IssueOptions o) => HandleIssueOptions(o),
                (ConfigurationOptions o) => Configure(o),
                ReturnInvalidArgsExitCode);

        private async Task<int> HandleIssueOptions(IssueOptions options)
        {
            if (!_configurationHandler.IsConfigurationValid())
                return ExitCode.InvalidConfiguration;

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

        private async Task<int> Create(CreateOptions options)
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

            if (options is CreateMergeRequestOptions createMergeRequestOptions)
            {
                var parameters = _parametersHandler.NegotiateCreateMergeRequestParameters(createMergeRequestOptions);
                if (parameters.IsSuccess)
                {
                    await _mergeRequestHandler.CreateMergeRequest(parameters.Value);
                }
            }

            return ExitCode.Success;
        }

        private Task<int> Configure(ConfigurationOptions options)
        {
            var parameters = _parametersHandler.NegotiateConfigurationParameters(options);
            _configurationHandler.StoreConfiguration(parameters);
            return Task.FromResult(ExitCode.Success);
        }

        private static Task<int> ReturnInvalidArgsExitCode(IEnumerable<Error> errors)
            => Task.FromResult(ExitCode.InvalidArguments);
    }
}
