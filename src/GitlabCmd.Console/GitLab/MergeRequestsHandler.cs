using System.Threading.Tasks;

namespace GitlabCmd.Console.GitLab
{
    public class MergeRequestsHandler
    {
        private readonly GitLabFacade _gitLabFacade;

        public MergeRequestsHandler(GitLabFacade gitLabFacade)
        {
            _gitLabFacade = gitLabFacade;
        }

        public async Task CreateMergeRequestAsync(CreateMergeRequestParameters parameters)
        {
            var mergeRequestResult = await _gitLabFacade.CreateMergeRequest(
                parameters.ProjectName,
                parameters.Title,
                parameters.SourceBranch,
                parameters.TargetBranch);
        }
    }
}
