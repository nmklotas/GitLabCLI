using System.Collections.Generic;
using System.Linq;

namespace GitlabCmd.Console.GitLab
{
    public class AddIssueParameters
    {
        public AddIssueParameters(
            string title, 
            string description, 
            string projectName,
            IEnumerable<string> labels = null)
        {
            Title = title;
            Description = description;
            ProjectName = projectName;
            Labels = labels != null ? labels.ToList() : new List<string>();
        }

        public string Title { get; }

        public string Description { get; }

        public string ProjectName { get; }

        public IReadOnlyList<string> Labels { get; }
    }
}
