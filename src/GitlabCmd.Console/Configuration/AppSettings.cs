namespace GitlabCmd.Console.Configuration
{
    public sealed class AppSettings
    {
        public string GitLabHostUrl { get; set; }

        public string GitLabUserName { get; set; }

        public string GitLabPassword { get; set; }

        public string GitLabAccessToken { get; set; }

        public string DefaultGitLabProject { get; set; }

        public string DefaultGitLabProjectForIssues { get; set; }

        public string DefaultGitLabProjectForMergeRequests { get; set; }

        public string DefaulGitLabIssueLabel { get; set; }
    }
}