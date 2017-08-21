using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("merge", HelpText = "Commands: list. Run merge [command] to learn more.")]
    [SubVerbs(typeof(ListMergesOptions))]
    public abstract class MergeOptions : ProjectOptions
    {
    }

    [Verb("list", HelpText = "Lists merge requests")]
    public sealed class ListMergesOptions : MergeOptions
    {
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
