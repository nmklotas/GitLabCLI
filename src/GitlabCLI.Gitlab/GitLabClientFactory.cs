using System.Threading.Tasks;
using GitLabApiClient;
using GitLabCLI.Utilities;

namespace GitLabCLI.GitLab
{
    public sealed class GitLabClientFactory
    {
        private readonly GitLabSettings _settings;
        private GitLabClient _client;

        public GitLabClientFactory(GitLabSettings settings) => _settings = settings;

        public async Task<GitLabClient> Create()
        {
            if (_client != null)
                return _client;

            if (!_settings.GitLabAccessToken.IsNullOrEmpty())
            {
                _client = new GitLabClient(_settings.GitLabHostUrl, _settings.GitLabAccessToken);
                return _client;
            }

            _client = new GitLabClient(_settings.GitLabHostUrl);
            await _client.LoginAsync(_settings.GitLabUserName, _settings.GitLabPassword);
            return _client;
        }
    }
}
