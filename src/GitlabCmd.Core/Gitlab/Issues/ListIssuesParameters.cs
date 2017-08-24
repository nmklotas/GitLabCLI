using System.Collections.Generic;
using GitLabCmd.Utilities;

namespace GitLabCmd.Core.GitLab.Issues
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

        public bool AssignedToCurrentUser { get; set; }

        public IList<string> Labels { get; set; } = new List<string>();
    }
}
