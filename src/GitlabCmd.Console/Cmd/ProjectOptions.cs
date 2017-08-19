using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    public abstract class ProjectOptions
    {
        [Option('p', "project", HelpText = "GitLab project name.")]
        public string Project { get; set; }
    }
}