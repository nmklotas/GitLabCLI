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

        public LaunchHandler(
            Parser parser, 
            ParametersHandler parametersHandler,
            ConfigurationHandler configurationHandler,
            GitLabIssueHandler issueHandler)
        {
            _parser = parser;
            _parametersHandler = parametersHandler;
            _configurationHandler = configurationHandler;
            _issueHandler = issueHandler;
        }

        public async Task<int> Launch(string[] args) => await _parser.
            ParseArguments<
                AddIssueOptions,
                GitlabCmdConfigurationOptions,
                CreateMergeRequestOptions,
                Task<int>>(args).
            MapResult(
                (AddIssueOptions options) => AddIssue(options),
                (GitlabCmdConfigurationOptions options) => Configure(options),
                (CreateMergeRequestOptions options) => CreateMergeRequest(options),
                HandleErrors);

        private async Task<int> AddIssue(AddIssueOptions options)
        {
            var parameters = _parametersHandler.GetAddIssueParameters(options);
            if (parameters.IsSuccess)
            {
                await _issueHandler.AddIssue(parameters.Value);
            }

            return ExitCode.Success;
        }

        private Task<int> Configure(GitlabCmdConfigurationOptions options)
        {
            var parameters = _parametersHandler.GetConfigurationParameters(options);
            _configurationHandler.Handle(parameters);
            return Task.FromResult(ExitCode.Success);
        }
             
        private Task<int> CreateMergeRequest(CreateMergeRequestOptions options) 
            => Task.FromResult(ExitCode.Success);

        private Task<int> HandleErrors(IEnumerable<Error> errors) 
            => Task.FromResult(ExitCode.InvalidArguments);
    }
}
