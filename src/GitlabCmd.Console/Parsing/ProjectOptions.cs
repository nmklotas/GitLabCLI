using CommandLine;

namespace GitlabCmd.Console.Parsing
{
    public abstract class ProjectOptions
    {
        [Option('p', "project", HelpText = "GitLab project name.")]
        public string Project { get; set; }
    }
}