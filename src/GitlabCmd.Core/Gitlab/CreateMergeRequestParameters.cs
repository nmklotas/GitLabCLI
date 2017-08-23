namespace GitlabCmd.Core.Gitlab
{
    public class CreateMergeRequestParameters
    {
        public CreateMergeRequestParameters(
            string projectName, 
            string sourceBranch,
            string targetBranch,
            string title, 
            string description,
            string assignee = null)
        {
            ProjectName = projectName;
            SourceBranch = sourceBranch;
            TargetBranch = targetBranch;
            Title = title;
            Assignee = assignee ?? "";
            Description = description;
        }

        public bool AssignedToCurrentUser { get; set; }

        public string TargetBranch { get; }

        public string ProjectName { get; }

        public string SourceBranch { get; }

        public string Title { get; }

        public string Assignee { get; }

        public string Description { get; }
    }
}
