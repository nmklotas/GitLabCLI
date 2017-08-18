using CommandLine;

namespace GitlabCmd.Console.Cmd
{
    [Verb("config")]
    public class GitlabCmdConfigurationOptions : ProjectOptions
    {
        [Option('t', "token", HelpText = "Authorization token of GitLab host")]
        public string Token { get; set; }

        [Option('h', "host", HelpText = "GitLab host url")]
        public string Host { get; set; }
    }
}