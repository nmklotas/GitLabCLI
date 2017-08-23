using GitlabCmd.Utilities;

namespace GitlabCmd.Core.Gitlab.Merges
{
    public sealed class ListMergesParameters
    {
        public ListMergesParameters(
            string project,
            MergeRequestState? state = null,
            string assignee = "")
        {
            Guard.NotEmpty(project, nameof(project));
            Project = project;
            State = state;
            Assignee = assignee ?? "";
        }

        public string Project { get; }

        public MergeRequestState? State { get; }

        public string Assignee { get; }

        public bool AssignedToCurrentUser { get; set; }
    }
}
