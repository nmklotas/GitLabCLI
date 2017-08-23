using System.Collections.Generic;
using System.Linq;

namespace GitlabCmd.Core.Gitlab
{
    public class CreateIssueParameters
    {
        public CreateIssueParameters(
            string title, 
            string description, 
            string projectName,
            string assigneeName,
            IEnumerable<string> labels = null)
        {
            Title = title;
            Description = description;
            ProjectName = projectName;
            AssigneeName = assigneeName;
            Labels = labels != null ? labels.ToList() : new List<string>();
        }

        public bool AssignToCurrentUser { get; set; }

        public string AssigneeName { get; }

        public string Title { get; }

        public string Description { get; }

        public string ProjectName { get; }

        public IReadOnlyList<string> Labels { get; }
    }
}
