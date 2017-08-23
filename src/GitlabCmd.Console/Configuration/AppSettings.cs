namespace GitLabCmd.Console.Configuration
{
    public sealed class AppSettings
    {
        public string GitLabHostUrl { get; set; }

        public string GitLabUserName { get; set; }

        public string GitLabPassword { get; set; }

        public string GitLabAccessToken { get; set; }

        public string DefaultProject { get; set; }

        public string DefaultIssuesProject { get; set; }

        public string DefaultMergesProject { get; set; }

        public string DefaulIssuesLabel { get; set; }
    }
}