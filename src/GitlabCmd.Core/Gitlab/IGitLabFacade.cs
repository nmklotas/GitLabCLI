using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitlabCmd.Core.Gitlab
{
    public interface IGitLabFacade
    {
        Task<Result<int>> CreateIssue(CreateIssueParameters parameters);

        Task<Result<int>> CreateMergeRequest(CreateMergeRequestParameters parameters);

        Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters);

        Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(ListMergesParameters parameters);
    }
}