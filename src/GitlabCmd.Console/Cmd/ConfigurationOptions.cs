using System.Threading.Tasks;
using CommandLine;
using GitlabCmd.Console.App;

namespace GitlabCmd.Console.Cmd
{
    [Verb("config")]
    public sealed class ConfigurationOptions : IVisitableOption
    {
        public Task Accept(LaunchOptionsVisitor visitor) => visitor.Visit(this);

        [Option('t', "token", HelpText = "Authorization token of GitLab host")]
        public string Token { get; set; }

        [Option('h', "host", HelpText = "GitLab host url")]
        public string Host { get; set; }

        [Option('u', "username", HelpText = "GitLab username")]
        public string Username { get; set; }

        [Option('p', "password", HelpText = "GitLab user password")]
        public string Password { get; set; }

        [Option('d', "default-project", 
            HelpText = 
                "Default GitLab project name." + 
                "Used for issues and merges.")]
        public string DefaultProject { get; set; }

        [Option('i', "default-issues-project", 
            HelpText = 
                "Default GitLab issue label." +
                "Used when creating & listing issues. This option overrides default-project for issues.")]
        public string DefaultIssuesProject { get; set; }

        [Option('m', "default-merges-project",
            HelpText =
                "Default GitLab project name." +
                "Used for merges. This option overrides default-project for merges.")]
        public string DefaultMergesProject { get; set; }

        [Option('l', "default-issues-label",
            HelpText = "Default GitLab issue labels. Used when creating & listing issues.")]
        public string DefaulIssuesLabel { get; set; }
    }
}