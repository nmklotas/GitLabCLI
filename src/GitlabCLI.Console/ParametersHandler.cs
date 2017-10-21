﻿using System.Collections.Generic;
using System.Linq;
using GitLabCLI.Console.Configuration;
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
        private readonly AppSettings _settings;

        public ParametersHandler(AppSettings settings) => _settings = settings;

        public Result<CreateIssueParameters> NegotiateCreateIssueParameters(CreateIssueOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<CreateIssueParameters>(project);

            return Result.Ok(new CreateIssueParameters(
                options.Title,
                options.Description,
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself,
                Labels = GetLabels(options.Labels)
            });
        }

        public Result<ListIssuesParameters> NegotiateListIssuesParameters(ListIssuesOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<ListIssuesParameters>(project);

            var outputFormat = ParseOutputFormat(options.Format);
            if (outputFormat.IsFailure)
                return Result.Fail<ListIssuesParameters>(outputFormat);

            return Result.Ok(new ListIssuesParameters(
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignedToMe,
                Format = outputFormat.Value,
                Labels = GetLabels(options.Labels)
            });
        }

        public Result<CreateMergeRequestParameters> NegotiateCreateMergeRequestParameters(CreateMergeRequestOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<CreateMergeRequestParameters>(project);

            return Result.Ok(new CreateMergeRequestParameters(
                options.Title,
                options.Source,
                options.Destination,
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself
            });
        }

        public Result<ListMergesParameters> NegotiateListMergesParameters(ListMergesOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<ListMergesParameters>(project);

            var state = ParseState(options.State);
            if (!state.HasValue)
                return Result.Fail<ListMergesParameters>($"State parameter: {options.State} is not supported." +
                                                          "Supported values are: opened|closed|merged");

            return Result.Ok(new ListMergesParameters(
                project.Value,
                state.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignedToMe
            });
        }

        public ConfigurationParameters NegotiateConfigurationParameters(ConfigurationOptions options) =>
            new ConfigurationParameters
            {
                Token = options.Token,
                Host = options.Host,
                DefaulIssuesLabel = options.DefaulIssuesLabel,
                DefaultIssuesProject = options.DefaultIssuesProject,
                DefaultMergesProject = options.DefaultMergesProject,
                DefaultProject = options.DefaultProject,
                Password = options.Password,
                Username = options.Username
            };

        private Result<string> GetProject(ProjectOptions options)
        {
            string projectName = !options.Project.IsNullOrEmpty() ?
                options.Project : _settings.DefaultProject;

            return projectName.IsNullOrEmpty() ?
                Result.Fail<string>(
                    "Project name is not provided and default is not set. " +
                    "You can set default project by running 'gitlab config --default-project {project}'") :
                Result.Ok(projectName);
        }

        private List<string> GetLabels(IEnumerable<string> labels)
        {
            var inputLabels = labels.
                SafeToList().
                Where(l => !l.IsNullOrEmpty()).
                ToList();

            if (inputLabels.Any())
                return inputLabels;

            string normalizedDefaultLabel = _settings.DefaulIssuesLabel.NormalizeSpaces();
            if (!normalizedDefaultLabel.IsNullOrEmpty())
                return new List<string> { normalizedDefaultLabel };

            return new List<string>();
        }

        private static MergeRequestState? ParseState(string state)
        {
            switch (state.NormalizeSpaces().ToUpperInvariant())
            {
                case "":
                    return MergeRequestState.Opened;
                case "OPENED":
                    return MergeRequestState.Opened;
                case "CLOSED":
                    return MergeRequestState.Closed;
                case "MERGED":
                    return MergeRequestState.Merged;
                default:
                    return null;
            }
        }

        private static Result<OutputFormat> ParseOutputFormat(string format)
        {
            if (format.NormalizeSpaces().EqualsIgnoringCase("rows") ||
                format.NormalizeSpaces().EqualsIgnoringCase(""))
            {
                return Result.Ok(OutputFormat.Rows);
            }
            else if (format.NormalizeSpaces().EqualsIgnoringCase("grid"))
            {
                return Result.Ok(OutputFormat.Grid);
            }
            else
            {
                return Result.Fail<OutputFormat>(
                    $"Output format {format} is not recognized. " +
                     "Available values are: rows, grid. Default is rows.");
            }
        }
    }
}
