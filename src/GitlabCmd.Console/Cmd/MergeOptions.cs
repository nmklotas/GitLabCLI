using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("merge", HelpText = "Commands: ls. Run merge [command] to learn more.")]
    [SubVerbs(typeof(ListMergesOptions))]
    public abstract class MergeOptions : ProjectOptions
    {
    }

    [Verb("list", HelpText = "Lists merge requests")]
    public sealed class ListMergesOptions : MergeOptions
    {
        [Value(0)]
        public MergeRequestState State { get; set; }

        [Option("assigned-to-me")]
        public bool AssignedToMe { get; set; }

        [Option('a', "assignee")]
        public string Assignee { get; set; }
    }

    public enum MergeRequestState
    {
        Opened,
        Closed,
        Merged
    }
}
