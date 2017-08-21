using NGitLab.Models;

namespace GitlabCmd.Console.GitLab
{
    public sealed class ListMergesParameters
    {
        public ListMergesParameters(string project, string assignee, MergeRequestState state)
        {
            Project = project;
            Assignee = assignee;
            State = state;
        }

        public bool AssignedToCurrentUser { get; set; }

        public string Project { get; }

        public string Assignee { get; }

        public MergeRequestState State { get; }
    }
}
