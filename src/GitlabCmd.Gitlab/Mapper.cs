using System.Collections.Generic;
using System.Linq;
using GitlabCmd.Core;

namespace GitlabCmd.Gitlab
{
    public sealed class Mapper
    {
        public Result<IReadOnlyList<Core.Gitlab.Issue>> Map(
            Result<IReadOnlyList<NGitLab.Models.Issue>> result)
        {
            return result.Map<IReadOnlyList<Core.Gitlab.Issue>>(r => r.Select(i => new Core.Gitlab.Issue
            {
                Assignee = i.Assignee.Name,
                Description = i.Description,
                Id = i.Id,
                Title = i.Title
            })
            .ToList());
        }

        public Result<IReadOnlyList<Core.Gitlab.MergeRequest>> Map(
            Result<IReadOnlyList<NGitLab.Models.MergeRequest>> result)
        {
            return result.Map<IReadOnlyList<Core.Gitlab.MergeRequest>>(r => r.Select(i => new Core.Gitlab.MergeRequest
            {
                Assignee = i.Assignee.Name,
                Id = i.Id,
                Title = i.Title
            })
            .ToList());
        }
    }
}
