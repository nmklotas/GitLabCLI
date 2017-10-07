namespace GitLabCLI.Core.Gitlab.Merges
{
    public sealed class MergeRequest
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Assignee { get; set; }
    }
}
