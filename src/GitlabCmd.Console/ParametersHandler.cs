using System.Collections.Generic;
using System.Linq;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.Parsing;
using GitlabCmd.Core;
using GitlabCmd.Core.Gitlab.Issues;
using GitlabCmd.Core.Gitlab.Merges;
using GitlabCmd.Utilities;

namespace GitlabCmd.Console
{
    public class ParametersHandler
    {
        private readonly AppSettings _settings;

        public ParametersHandler(AppSettings settings) => _settings = settings;

        public Result<CreateIssueParameters> NegotiateAddIssueParameters(CreateIssueOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<CreateIssueParameters>(project);

            var labels = GetLabels(options.Labels);

            var parameters = new CreateIssueParameters(
                options.Title,
                options.Description,
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself,
                Labels = labels.ToList()
            };

            return Result.Ok(parameters);
        }

        public Result<ListIssuesParameters> NegotiateListIssuesParameters(ListIssuesOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<ListIssuesParameters>(project);

            var labels = GetLabels(options.Labels);

            var parameters = new ListIssuesParameters(
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignedToMe,
                Labels = labels.ToList()
            };

            return Result.Ok(parameters);
        }

        public Result<CreateMergeRequestParameters> NegotiateCreateMergeRequestParameters(CreateMergeRequestOptions options)
        {
            var project = GetProject(options);
            if (project.IsFailure)
                return Result.Fail<CreateMergeRequestParameters>(project);

            var parameters = new CreateMergeRequestParameters(
                options.Title,
                options.Source,
                options.Destination,
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself
            };

            return Result.Ok(parameters);
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

            var parameters = new ListMergesParameters(
                project.Value,
                state.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignedToMe
            };

            return Result.Ok(parameters);
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
            string projectName = options.Project.IsNotNullOrEmpty() ?
                options.Project : _settings.DefaultProject;

            return projectName.IsNullOrEmpty() ?
                Result.Fail<string>("Project name is not provided and default is not set") :
                Result.Ok(projectName);
        }

        private IEnumerable<string> GetLabels(IEnumerable<string> labels)
        {
            var inputLabels = labels.NormalizeSpaces().ToList();
            if (inputLabels.Any(l => l.IsNotNullOrEmpty()))
                return inputLabels;

            string normalizedDefaultLabel = _settings.DefaulIssuesLabel.NormalizeSpaces();
            if (normalizedDefaultLabel.IsNotNullOrEmpty())
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
    }
}
