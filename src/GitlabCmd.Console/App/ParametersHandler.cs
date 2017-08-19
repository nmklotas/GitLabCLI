using System.Linq;
using GitlabCmd.Console.Cmd;
using GitlabCmd.Console.Configuration;
using GitlabCmd.Console.GitLab;
using GitlabCmd.Console.Utilities;

namespace GitlabCmd.Console.App
{
    public class ParametersHandler
    {
        private readonly AppSettings _settings;

        public ParametersHandler(AppSettings settings) => _settings = settings;

        public Result<AddIssueParameters> GetAddIssueParameters(CreateIssueOptions options)
        {
            string projectName = options.Project.IsNotEmpty() ? 
                options.Project : _settings.DefaultGitLabProject;

            if (projectName.IsEmpty())
                return Result.Fail<AddIssueParameters>("Project name is not provided and default is not set");

            var labels = options.Labels.Any() ? 
                options.Labels : new[] { _settings.DefaultIssueLabel };

            var parameters = new AddIssueParameters(
                options.Title,
                options.Description,
                projectName,
                options.Assignee,
                labels)
            {
                AssignToCurrentUser = options.AssignMyself
            };

            return Result.Ok(parameters);
        }

        public ConfigurationParameters GetConfigurationParameters(GitlabCmdConfigurationOptions options) 
            => null;
    }
}
