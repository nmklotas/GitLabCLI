using GitLabCLI.Utilities;

namespace GitLabCLI.Core.Gitlab.Issues
{
    public sealed class CloseIssueParameters
    {
        public CloseIssueParameters(
            string project,
            int issueId)
        {
            Guard.NotEmpty(project, nameof(project));
            Project = project;
            IssueId = issueId;
        }

        public string Project { get; }

        public int IssueId { get; }
    }
}
