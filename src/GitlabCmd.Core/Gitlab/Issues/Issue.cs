namespace GitLabCmd.Core.GitLab.Issues
{
    public sealed class Issue
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Assignee { get; set; }
    }
}
