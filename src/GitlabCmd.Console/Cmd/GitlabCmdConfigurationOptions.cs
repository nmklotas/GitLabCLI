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

        [Option('u', "username", HelpText = "GitLab username")]
        public string Username { get; set; }

        [Option('p', "password", HelpText = "GitLab user password")]
        public string Password { get; set;}
    }
}