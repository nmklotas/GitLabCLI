﻿using System.Collections.Generic;
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
            ParseVerbs<
                CreateOptions,
                IssueOptions,
                ConfigurationOptions,
                Task<int>>(args).
            MapResult(
                (CreateOptions options) => Create(options),
                (IssueOptions options) => HandleIssueOptions(options),
                (ConfigurationOptions options) => Configure(options),
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

            return ExitCode.Success;
        }

        private Task<int> Configure(ConfigurationOptions options)
        {
            var parameters = _parametersHandler.GetConfigurationParameters(options);
            _configurationHandler.StoreConfiguration(parameters);
            return Task.FromResult(ExitCode.Success);
        }

        private static Task<int> ReturnInvalidArgsExitCode(IEnumerable<Error> errors)
            => Task.FromResult(ExitCode.InvalidArguments);
    }
}
