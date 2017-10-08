using Newtonsoft.Json;

namespace GitLabCLI.Console.Configuration
{
    public sealed class AppSettings
    {
        [JsonProperty("host_url")]
        public string GitLabHostUrl { get; set; }

        [JsonProperty("username")]
        public string GitLabUserName { get; set; }

        [JsonProperty("password")]
        public string GitLabPassword { get; set; }

        [JsonProperty("access_token")]
        public string GitLabAccessToken { get; set; }

        [JsonProperty("default_project")]
        public string DefaultProject { get; set; }

        [JsonProperty("default_issues_project")]
        public string DefaultIssuesProject { get; set; }

        [JsonProperty("default_merges_project")]
        public string DefaultMergesProject { get; set; }

        [JsonProperty("default_issues_label")]
        public string DefaulIssuesLabel { get; set; }
    }
}