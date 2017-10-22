using GitLabCLI.Utilities;

namespace GitLabCLI.Core
{
    public class BrowseParameters
    {
        public BrowseParameters(string project, int issueId)
        {
            Guard.NotEmpty(project, nameof(project));
            Project = project;
            IssueId = issueId;
        }

        public string Project { get; }

        public int IssueId { get; }
    }
}
