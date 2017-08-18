using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        private readonly HttpRequestor _requestor;

        public GitLabClientEx(string hostUrl, string privateToken = "") : base(hostUrl, privateToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(hostUrl)
            };

            client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateToken);

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
    }
}
