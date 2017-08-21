using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NGitLab;
using NGitLab.Impl;
using NGitLab.Models;

namespace GitlabCmd.Console.GitLab
{
    /// <summary>
    /// Workaround limitations of NGitLab package
    /// </summary>
    public sealed class GitLabClientEx : GitLabClient
    {
        private const string _privateToken = "PRIVATE-TOKEN";
        private readonly HttpRequestor _requestor;

        public GitLabClientEx(string hostUrl, string privateToken = "") : base(hostUrl, privateToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(hostUrl)
            };

            client.DefaultRequestHeaders.Add(_privateToken, privateToken);

            _requestor = new HttpRequestor
            {
                HostUrl = hostUrl,
                Client = client
            };
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            try
            {
                return (await _requestor.Get<User[]>($"/users?username={name}")).FirstOrDefault();
            }
            catch (GitLabException)
            {
                return null;
            }
        }

        public async Task<MergeRequest> CreateMergeAsync(int projectId, MergeRequestCreate mergeRequest)
        {
            try
            {
                return await GetMergeRequest(projectId).CreateAsync(mergeRequest);
            }
            catch (JsonException)
            {
                throw new GitLabException(
                    "Failed to create merge request. Ensure if the merge requests does not already exists");
            }
        }

        public new async Task<Session> LoginAsync(string username, string password)
        {
            var session = await base.LoginAsync(username, password);

            if (_requestor.Client.DefaultRequestHeaders.Contains(_privateToken))
                _requestor.Client.DefaultRequestHeaders.Remove(_privateToken);

            _requestor.Client.DefaultRequestHeaders.Add(_privateToken, session.PrivateToken);
            return session;
        }

        public async Task DeleteMergeRequest(int projectId, int mergeRequestId) => 
            await _requestor.Delete($"/projects/{projectId}/merge_requests/{mergeRequestId}");
    }
}
