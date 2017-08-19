using System.Collections.Generic;
using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("issue", HelpText = "Commands: ls. Run issue [command] to learn more.")]
    [SubVerbs(typeof(ListIssuesOptions))]
    public abstract class IssueOptions : ProjectOptions
    {
    }

    [Verb("ls", HelpText = "Lists issues")]
    public sealed class ListIssuesOptions : IssueOptions
    {
        [Option("assigned-to-me")]
        public bool AssignedToMe { get; set; }

        [Option('a', "assignee")]
        public string Assignee { get; set; }

        [Option('l', "labels", Separator = ',')]
        public IEnumerable<string> Labels { get; set; }
    }
}
