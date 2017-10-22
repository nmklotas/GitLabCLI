using System;

namespace GitLabCLI.Core.Gitlab.Issues
{
    public sealed class Issue
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Assignee { get; set; }

        public string Author { get; set; }

        public DateTime CreatedAt { get; set; }

        public string WebUrl { get; set; }
    }
}
