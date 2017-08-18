using System.Collections.Generic;
using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("add-issue", HelpText = "Creates new GitLab issue.")]
    public class AddIssueOptions : ProjectOptions
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
}