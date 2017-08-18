using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("create-merge-request", HelpText = "Creates new GitLab issue.")]
    public class CreateMergeRequestOptions : ProjectOptions
    {
        [Option('s', "source", HelpText = "Source branch", Required = true)]
        public string SourceBranch { get; set; }

        [Option('d', "destination", HelpText = "Destination branch")]
        public string Description { get; set; }

        [Option('t', "title", HelpText = "Title of merge request")]
        public string Title { get; set; }
    }
}