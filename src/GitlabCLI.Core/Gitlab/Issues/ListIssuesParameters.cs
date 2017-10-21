using System.Collections.Generic;
using GitLabCLI.Utilities;

namespace GitLabCLI.Core.Gitlab.Issues
{
    public sealed class ListIssuesParameters
    {
        public ListIssuesParameters(
            string project,
            string assignee = "")
        {
            Guard.NotEmpty(project, nameof(project));
            Project = project;
            Assignee = assignee ?? "";
        }

        public string Project { get; }

        public string Assignee { get; }

        public OutputFormat Format { get; set; }

        public bool AssignedToCurrentUser { get; set; }

        public IssueState IssueState { get; set; }

        public IList<string> Labels { get; set; } = new List<string>();
    }
}
