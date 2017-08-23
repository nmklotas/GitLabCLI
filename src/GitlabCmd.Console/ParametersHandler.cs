using System.Collections.Generic;
using System.Linq;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.Parsing;
using GitlabCmd.Core;
using GitlabCmd.Core.Gitlab;
using GitlabCmd.Utilities;

namespace GitlabCmd.Console
{
    public class ParametersHandler
    {
        private readonly AppSettings _settings;

        public ParametersHandler(AppSettings settings) => _settings = settings;

        public Result<CreateIssueParameters> NegotiateAddIssueParameters(CreateIssueOptions options)
        {
            var projectName = GetProjectName(options);
            if (projectName.IsFailure)
                return Result.Fail<CreateIssueParameters>(projectName);

            var labels = GetLabels(options.Labels);

            var parameters = new CreateIssueParameters(
                options.Title,
                options.Description,
                projectName.Value,
                options.Assignee,
                labels)
            {
                AssignToCurrentUser = options.AssignMyself
            };

            return Result.Ok(parameters);
        }

        public Result<ListIssuesParameters> NegotiateListIssuesParameters(ListIssuesOptions options)
        {
            var projectName = GetProjectName(options);
            if (projectName.IsFailure)
                return Result.Fail<ListIssuesParameters>(projectName);

            var labels = GetLabels(options.Labels);

            var parameters = new ListIssuesParameters(
                options.Assignee,
                projectName.Value,
                labels)
            {
                AssignedToCurrentUser = options.AssignedToMe
            };

            return Result.Ok(parameters);
        }

        public Result<CreateMergeRequestParameters> NegotiateCreateMergeRequestParameters(CreateMergeRequestOptions options)
        {
            var projectName = GetProjectName(options);
            if (projectName.IsFailure)
                return Result.Fail<CreateMergeRequestParameters>(projectName);

            var parameters = new CreateMergeRequestParameters(
                projectName.Value,
                options.Source,
                options.Destination,
                options.Title,
                "", //TODO: description is not supported currently..
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself
            };

            return Result.Ok(parameters);
        }

        public Result<ListMergesParameters> NegotiateListMergesParameters(ListMergesOptions options)
        {
            var projectName = GetProjectName(options);
            if (projectName.IsFailure)
                return Result.Fail<ListMergesParameters>(projectName);

            var mappedState = Map(options.State);
            if (!mappedState.HasValue)
                return Result.Fail<ListMergesParameters>($"State parameter: {options.State} is not supported." +
                                                          "Supported values are: opened|closed|merged");

            var parameters = new ListMergesParameters(
                projectName.Value,
                options.Assignee,
                mappedState.Value)
            {
                AssignedToCurrentUser = options.AssignedToMe
            };

            return Result.Ok(parameters);

            MergeRequestState? Map(string state)
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

        private Result<string> GetProjectName(ProjectOptions options)
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
    }
}
