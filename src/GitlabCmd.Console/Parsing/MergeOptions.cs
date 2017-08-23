using System.Threading.Tasks;
using CommandLine;

namespace GitLabCmd.Console.Parsing
{
    [Verb("merge", HelpText = "Commands: create, list. Run merge [command] to learn more.")]
    [SubVerbs(typeof(CreateMergeRequestOptions), typeof(ListMergesOptions))]
    public abstract class MergeOptions : ProjectOptions
    {
    }

    [Verb("create", HelpText = "Creates merge request")]
    public sealed class CreateMergeRequestOptions : MergeOptions, IVisitableOption
    {
        public Task Accept(LaunchOptionsVisitor visitor) => visitor.Visit(this);

        [Option('s', "source", HelpText = "Source branch", Required = true)]
        public string Source { get; set; }

        [Option('d', "destination", HelpText = "Destination branch", Required = true)]
        public string Destination { get; set; }

        [Option('t', "title", HelpText = "Title of merge request", Required = true)]
        public string Title { get; set; }

        [Option('a', "assignee")]
        public string Assignee { get; set; }

        [Option("assign-myself")]
        public bool AssignMyself { get; set; }
    }

    [Verb("list", HelpText = "Lists merge requests")]
    public sealed class ListMergesOptions : MergeOptions, IVisitableOption
    {
        public Task Accept(LaunchOptionsVisitor visitor) => visitor.Visit(this);

        [Value(
            0,
            MetaName = "State",
            Default = "opened",
            HelpText = "Merge request state. Can be opened|merged|closed.")]
        public string State { get; set; }

        [Option("assigned-to-me")]
        public bool AssignedToMe { get; set; }

        [Option('a', "assignee")]
        public string Assignee { get; set; }
    }
}
