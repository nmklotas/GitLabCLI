using System.Collections.Generic;
using System.Linq;

namespace GitlabCmd.Console.GitLab
{
    public class ListIssuesParameters
    {
        public ListIssuesParameters(
            string assignee, 
            string projectName,
            IEnumerable<string> labels = null)
        {
            Assignee = assignee;
            ProjectName = projectName;
            Labels = labels != null ? labels.ToList() : new List<string>();
        }

        public bool AssignedToCurrentUser { get; set; }

        public string Assignee { get;  }

        public string ProjectName { get; }

        public IReadOnlyList<string> Labels { get; }
    }
}
