using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitLabCmd.Core;
using GitLabCmd.Core.GitLab;
using GitLabCmd.Core.GitLab.Issues;
using GitLabCmd.Core.GitLab.Merges;
using NGitLab.Impl;

namespace GitLabCmd.GitLab
{
    public class GitLabFacade : IGitLabFacade
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

        public async Task<Result<int>> CreateMergeRequestAsync(CreateMergeRequestParameters parameters)
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