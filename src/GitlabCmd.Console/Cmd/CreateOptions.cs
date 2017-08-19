using System.Collections.Generic;
using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("create", HelpText = "Commands: issue, merge. Run create [command] to learn more.")]
    [SubVerbs(
        typeof(CreateMergeRequestOptions), 
        typeof(CreateIssueOptions))]
    public abstract class CreateOptions : ProjectOptions
    {
    }

    [Verb("issue", HelpText = "Creates new issue")]
    public sealed class CreateIssueOptions : CreateOptions
    {
        [Option('t', "title", HelpText = "Title of issue", Required = true)]
        public string Title { get; set; }

        [Option('d', "description", HelpText = "Description of issue")]
        public string Description { get; set; }

        [Option('l', "labels", Separator = ',')]
        public IEnumerable<string> Labels { get; set; }

        [Option('a', "assignee")]
        public string Assignee { get; set; }

        [Option("assign-myself")]
        public bool AssignMyself { get; set; }
    }

    [Verb("merge", HelpText = "Creates merge request")]
    public sealed class CreateMergeRequestOptions : CreateOptions
    {
        [Option('s', "source", HelpText = "Source branch")]
        public string Source { get; set; }

        [Option('d', "destination", HelpText = "Destination branch")]
        public string Destination { get; set; }

        [Option('t', "title", HelpText = "Title of merge request")]
        public string Title { get; set; }

        [Option('a', "assignee")]
        public string Assignee { get; set; }
    }
}