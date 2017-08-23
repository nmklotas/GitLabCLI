using GitlabCmd.Utilities;

namespace GitlabCmd.Core.Gitlab.Merges
{
    public class CreateMergeRequestParameters
    {
        public CreateMergeRequestParameters(
            string title, 
            string sourceBranch,
            string targetBranch,
            string project,
            string assignee = "")
        {
            Guard.NotEmpty(title, nameof(title));
            Guard.NotEmpty(sourceBranch, nameof(sourceBranch));
            Guard.NotEmpty(targetBranch, nameof(targetBranch));
            Guard.NotEmpty(project, nameof(project));
            Title = title;
            SourceBranch = sourceBranch;
            TargetBranch = targetBranch;
            Project = project;
            Assignee = assignee ?? "";
        }

        public string Title { get; }

        public string SourceBranch { get; }

        public string TargetBranch { get; }

        public string Project { get; }

        public string Assignee { get; }

        public bool AssignedToCurrentUser { get; set; }
    }
}
