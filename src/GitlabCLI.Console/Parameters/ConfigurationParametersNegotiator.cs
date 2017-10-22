using GitLabCLI.Console.Configuration;
using GitLabCLI.Console.Parsing;

namespace GitLabCLI.Console.Parameters
{
    public class ConfigurationParametersNegotiator
    {
        public ConfigurationParameters NegotiateConfigurationParameters(ConfigurationOptions options) => new ConfigurationParameters
        {
            Token = options.Token,
            Host = options.Host,
            DefaulIssuesLabel = options.DefaulIssuesLabel,
            DefaultIssuesProject = options.DefaultIssuesProject,
            DefaultMergesProject = options.DefaultMergesProject,
            DefaultProject = options.DefaultProject,
            Password = options.Password,
            Username = options.Username
        };
    }
}
