using System;
using System.Collections.Generic;
using System.Linq;
using GitlabCmd.Console.Cmd;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
using GitlabCmd.Console.Utilities;
using Result = GitlabCmd.Console.Utilities.Result;

namespace GitlabCmd.Console.App
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

            var parameters = new ListMergesParameters(
                projectName.Value,
                options.Assignee,
                mappedState)
            {
                AssignedToCurrentUser = options.AssignedToMe
            };
            
            return Result.Ok(parameters);

            NGitLab.Models.MergeRequestState Map(MergeRequestState state)
            {
                switch (state)
                {
                    case MergeRequestState.Opened:
                        return NGitLab.Models.MergeRequestState.opened;
                    case MergeRequestState.Closed:
                        return NGitLab.Models.MergeRequestState.closed;
                    case MergeRequestState.Merged:
                        return NGitLab.Models.MergeRequestState.merged;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public ConfigurationParameters NegotiateConfigurationParameters(ConfigurationOptions options) => 
            new ConfigurationParameters
            {
                Token = options.Token,
                Host = options.Token,
                DefaulIssuesLabel = options.DefaulIssueLabel,
                DefaultIssuesProject = options.DefaultIssuesProject,
                DefaultMergesProject = options.DefaultMergesProject,
                DefaultProject = options.DefaultProject,
                Password = options.Password,
                Username = options.Username
            };

        private Result<string> GetProjectName(ProjectOptions options)
        {
            string projectName = options.Project.IsNotEmpty() ?
                options.Project : _settings.DefaultProject;

            return projectName.IsEmpty() ?
                Result.Fail<string>("Project name is not provided and default is not set") :
                Result.Ok(projectName);
        }

        private IEnumerable<string> GetLabels(IEnumerable<string> labels)
        {
            var inputLabels = labels.ToList();
            return inputLabels.Any() ? inputLabels.Normalize() : new[] { _settings.DefaulIssuesLabel };
        }
    }
}
