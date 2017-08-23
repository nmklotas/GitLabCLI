using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabCmd.Core.GitLab.Issues;
using GitLabCmd.Core.GitLab.Merges;

namespace GitLabCmd.Core.GitLab
{
    public interface IGitLabFacade
    {
        Task<Result<int>> CreateIssue(CreateIssueParameters parameters);

        Task<Result<int>> CreateMergeRequestAsync(CreateMergeRequestParameters parameters);

        Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters);

        Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(ListMergesParameters parameters);
    }
}