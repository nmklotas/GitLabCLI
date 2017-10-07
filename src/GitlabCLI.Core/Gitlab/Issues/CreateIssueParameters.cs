using System.Collections.Generic;
using GitLabCLI.Utilities;

namespace GitLabCLI.Core.Gitlab.Issues
{
    public sealed class CreateIssueParameters
    {
        public CreateIssueParameters(
           string title,
           string description,
           string project,
           string assignee = "")
        {
            Guard.NotEmpty(title, nameof(title));
            Guard.NotEmpty(project, nameof(project));
            Title = title ?? "";
            Description = description ?? "";
            Project = project ?? "";
            Assignee = assignee ?? "";
        }

        public string Title { get; }

        public string Description { get; }

        public string Project { get; }

        public string Assignee { get; }

        public bool AssignedToCurrentUser { get; set; }

        public IList<string> Labels { get; set; } = new List<string>();
    }
}
