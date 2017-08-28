using System;
using System.Collections.Generic;
using System.Linq;
using GitLabCLI.Core;
using MergeRequestState = GitLabCLI.Core.Gitlab.Merges.MergeRequestState;
using Issue = GitLabCLI.Core.Gitlab.Issues.Issue;
using MergeRequest = GitLabCLI.Core.Gitlab.Merges.MergeRequest;

namespace GitLabCLI.GitLab
{
    public sealed class Mapper
    {
        public Result<IReadOnlyList<Issue>> Map(Result<IReadOnlyList<GitLabApiClient.Models.Issues.Issue>> result)
        {
            return result.Map<IReadOnlyList<Issue>>(r => r.Select(i => new Issue
            {
                Assignee = i.Assignee?.Name,
                Description = i.Description,
                Id = i.Iid,
                Title = i.Title
            })
            .ToList());
        }

        public Result<IReadOnlyList<MergeRequest>> Map(Result<IReadOnlyList<GitLabApiClient.Models.Merges.MergeRequest>> result)
        {
            return result.Map<IReadOnlyList<MergeRequest>>(r => r.Select(i => new MergeRequest
            {
                Assignee = i.Assignee?.Name,
                Id = i.Iid,
                Title = i.Title
            })
            .ToList());
        }

        public GitLabApiClient.Models.Merges.MergeRequestState Map(MergeRequestState state)
        {
            switch (state)
            {
                case MergeRequestState.Opened:
                    return GitLabApiClient.Models.Merges.MergeRequestState.Opened;
                case MergeRequestState.Merged:
                    return GitLabApiClient.Models.Merges.MergeRequestState.Merged;
                case MergeRequestState.Closed:
                    return GitLabApiClient.Models.Merges.MergeRequestState.Closed;
                default:
                    throw new NotSupportedException($"State {state} is not supported");
            }
        }
    }
}
