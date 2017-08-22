using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using GitlabCmd.Console.App;

namespace GitlabCmd.Console.Cmd
{
    [Verb("issue", HelpText = "Commands: create, list. Run issue [command] to learn more.")]
    [SubVerbs(typeof(CreateIssueOptions), typeof(ListIssuesOptions))]
    public abstract class IssueOptions : ProjectOptions, IVisitableOption
    {
        public Task Accept(LaunchOptionsVisitor visitor) => visitor.Visit(this);
    }

    [Verb("create", HelpText = "Creates new issue")]
    public sealed class CreateIssueOptions : IssueOptions
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

    [Verb("list", HelpText = "Lists issues")]
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
