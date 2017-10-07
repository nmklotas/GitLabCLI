using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Core.Gitlab.Merges;

namespace GitLabCLI.GitLab
{
    public sealed class GitLabFacade : IGitLabFacade
    {
        private readonly GitLabIssuesFacade _issuesFacade;
        private readonly GitLabMergesFacade _mergesFacade;
        private readonly Mapper _mapper;

        public GitLabFacade(
            GitLabIssuesFacade issuesFacade,
            GitLabMergesFacade mergesFacade,
            Mapper mapper)
        {
            _issuesFacade = issuesFacade;
            _mergesFacade = mergesFacade;
            _mapper = mapper;
        }

        public async Task<Result<int>> CreateMergeRequest(CreateMergeRequestParameters parameters)
        {
            return await SafeGetResult(() => _mergesFacade.CreateMergeRequest(parameters));
        }

        public async Task<Result<IReadOnlyList<MergeRequest>>> ListMergeRequests(ListMergesParameters parameters)
        {
            return _mapper.Map(await SafeGetResult(() => _mergesFacade.ListMergeRequests(parameters)));
        }

        public async Task<Result<int>> CreateIssue(CreateIssueParameters parameters)
        {
            return await SafeGetResult(() => _issuesFacade.CreateIssue(parameters));
        }

        public async Task<Result<IReadOnlyList<Issue>>> ListIssues(ListIssuesParameters parameters)
        {
            return _mapper.Map(await SafeGetResult(() => _issuesFacade.ListIssues(parameters)));
        }

        private static async Task<Result<T>> SafeGetResult<T>(Func<Task<Result<T>>> resultDelegate)
        {
            try
            {
                return await resultDelegate();
            }
            catch (OperationCanceledException)
            {
                return Result.Fail<T>("Request timed out");
            }
            catch (GitLabException ex)
            {
                return Result.Fail<T>($"Request failed. {ex.Message}");
            }
        }
    }
}