namespace GitlabCmd.Console.GitLab
{
    public class CreateMergeRequestParameters
    {
        public CreateMergeRequestParameters(
            string projectName, 
            string sourceBranch,
            string targetBranch,
            string title, 
            string description)
        {
            ProjectName = projectName;
            SourceBranch = sourceBranch;
            TargetBranch = targetBranch;
            Title = title;
            Description = description;
        }

        public string TargetBranch { get; }

        public string ProjectName { get; }

        public string SourceBranch { get; }

        public string Title { get; }

        public string Description { get; }
    }
}
