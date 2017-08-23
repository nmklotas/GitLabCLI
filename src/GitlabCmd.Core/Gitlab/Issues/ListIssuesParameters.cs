using System.Collections.Generic;
using System.Linq;

namespace GitlabCmd.Core.Gitlab.Issues
{
    public class ListIssuesParameters
    {
        public ListIssuesParameters(
            string assignee, 
            string project,
            IEnumerable<string> labels = null)
        {
            Assignee = assignee;
            Project = project;
            Labels = labels != null ? labels.ToList() : new List<string>();
        }

        public bool AssignedToCurrentUser { get; set; }

        public string Assignee { get;  }

        public string Project { get; }

        public IReadOnlyList<string> Labels { get; }
    }
}
