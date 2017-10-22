using System;
using System.Collections.Generic;
using System.Linq;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.MergeRequests.Requests;
using GitLabCLI.Core;
using MergeRequestState = GitLabCLI.Core.Gitlab.Merges.MergeRequestState;
using Issue = GitLabCLI.Core.Gitlab.Issues.Issue;
using MergeRequest = GitLabCLI.Core.Gitlab.Merges.MergeRequest;

namespace GitLabCLI.GitLab
{
    public sealed class Mapper
    {
        public Result<IReadOnlyList<Issue>> Map(Result<IReadOnlyList<GitLabApiClient.Models.Issues.Responses.Issue>> result)
        {
            return result.Map<IReadOnlyList<Issue>>(r => r.Select(i => new Issue
            {
                Assignee = i.Assignee?.Name,
                Description = i.Description,
                Id = i.Iid,
                Title = i.Title,
                Author = i.Author.Name,
                CreatedAt = i.CreatedAt,
                WebUrl = i.WebUrl
            })
            .ToList());
        }

        public Result<IReadOnlyList<MergeRequest>> Map(Result<IReadOnlyList<GitLabApiClient.Models.MergeRequests.Responses.MergeRequest>> result)
        {
            return result.Map<IReadOnlyList<MergeRequest>>(r => r.Select(i => new MergeRequest
            {
                Assignee = i.Assignee?.Name,
                Id = i.Iid,
                Title = i.Title,
                Author = i.Author.Name,
                CreatedAt = i.CreatedAt
            })
            .ToList());
        }

        public QueryMergeRequestState Map(MergeRequestState state)
        {
            switch (state)
            {
                case MergeRequestState.Opened:
                    return QueryMergeRequestState.Opened;
                case MergeRequestState.Merged:
                    return QueryMergeRequestState.Merged;
                case MergeRequestState.Closed:
                    return QueryMergeRequestState.Closed;
                case MergeRequestState.All:
                    return QueryMergeRequestState.All;
                default:
                    throw new NotSupportedException($"State {state} is not supported");
            }
        }

        public IssueState Map(Core.Gitlab.Issues.IssueState state)
        {
            switch (state)
            {
                case Core.Gitlab.Issues.IssueState.Opened:
                    return IssueState.Opened;
                case Core.Gitlab.Issues.IssueState.Closed:
                    return IssueState.Closed;
                case Core.Gitlab.Issues.IssueState.All:
                    return IssueState.All;
                default:
                    throw new NotSupportedException($"State {state} is not supported");
            }
        }
    }
}
