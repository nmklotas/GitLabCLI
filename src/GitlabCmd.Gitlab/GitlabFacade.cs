using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitlabCmd.Core;
using GitlabCmd.Core.Gitlab;
using GitlabCmd.Core.Gitlab.Issues;
using GitlabCmd.Core.Gitlab.Merges;
using NGitLab.Impl;

namespace GitlabCmd.Gitlab
{
    public class GitLabFacade : IGitLabFacade
    {
        private readonly GitlabIssuesFacade _issuesFacade;
        private readonly GitlabMergesFacade _mergesFacade;
        private readonly Mapper _mapper;

        public GitLabFacade(
            GitlabIssuesFacade issuesFacade,
            GitlabMergesFacade mergesFacade,
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