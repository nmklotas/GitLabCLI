namespace GitlabCmd.Console.App
{
    public class ConfigurationParameters
    {
        public ConfigurationParameters(string token, string host, string username, string password)
        {
            Token = token;
            Host = host;
            Username = username;
            Password = password;
        }

        public string Token { get; }

        public string Host { get; }

        public string Username { get; }

        public string Password { get; }
    }
}
