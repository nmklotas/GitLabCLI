using System;
using System.Collections.Generic;
using System.Linq;
using GitlabCmd.Core;
using MergeRequestState = GitlabCmd.Core.Gitlab.Merges.MergeRequestState;
using Issue = GitlabCmd.Core.Gitlab.Issues.Issue;
using MergeRequest = GitlabCmd.Core.Gitlab.Merges.MergeRequest;

namespace GitlabCmd.Gitlab
{
    public sealed class Mapper
    {
        public Result<IReadOnlyList<Issue>> Map(
            Result<IReadOnlyList<NGitLab.Models.Issue>> result)
        {
            return result.Map<IReadOnlyList<Issue>>(r => r.Select(i => new Issue
            {
                Assignee = i.Assignee.Name,
                Description = i.Description,
                Id = i.Id,
                Title = i.Title
            })
            .ToList());
        }

        public Result<IReadOnlyList<MergeRequest>> Map(
            Result<IReadOnlyList<NGitLab.Models.MergeRequest>> result)
        {
            return result.Map<IReadOnlyList<MergeRequest>>(r => r.Select(i => new MergeRequest
            {
                Assignee = i.Assignee.Name,
                Id = i.Id,
                Title = i.Title
            })
            .ToList());
        }

        public NGitLab.Models.MergeRequestState Map(MergeRequestState state)
        {
            switch (state)
            {
                case MergeRequestState.Opened:
                    return NGitLab.Models.MergeRequestState.opened;
                case MergeRequestState.Merged:
                    return NGitLab.Models.MergeRequestState.merged;
                case MergeRequestState.Closed:
                    return NGitLab.Models.MergeRequestState.closed;
                default:
                    throw new NotSupportedException($"State {state} is not supported");
            }
        }
    }
}
