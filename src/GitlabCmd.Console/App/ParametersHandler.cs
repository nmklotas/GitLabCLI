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

            var labels = GetLabels(options);

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

            var labels = GetLabels(options);

            var parameters = new ListIssuesParameters(
                options.Assignee,
                projectName.Value,
                labels)
            {
                AssignedToCurrentUser = options.AssignedToMe
            };

            return Result.Ok(parameters);
        }

        public ConfigurationParameters GetConfigurationParameters(GitlabCmdConfigurationOptions options)
            => null;

        private Result<string> GetProjectName(ProjectOptions options)
        {
            string projectName = options.Project.IsNotEmpty() ?
                options.Project : _settings.DefaultGitLabProject;

            return projectName.IsEmpty() ?
                Result.Fail<string>("Project name is not provided and default is not set") :
                Result.Ok(projectName);
        }

        private IEnumerable<string> GetLabels(CreateIssueOptions options) =>
            options.Labels.Any() ? options.Labels : new[] { _settings.DefaulGitLabIssueLabel };

        private IEnumerable<string> GetLabels(ListIssuesOptions options) =>
            options.Labels.Any() ? options.Labels : new[] { _settings.DefaulGitLabIssueLabel };
    }
}
