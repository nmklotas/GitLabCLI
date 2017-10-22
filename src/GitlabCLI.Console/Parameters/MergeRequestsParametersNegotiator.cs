using GitLabCLI.Console.Parsing;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Merges;
using GitLabCLI.Utilities;
using static GitLabCLI.Core.Result;

namespace GitLabCLI.Console.Parameters
{
    public class MergeRequestsParametersNegotiator : ProjectParametersNegotiator
    {
        public Result<CreateMergeRequestParameters> NegotiateCreateMergeRequestParameters(CreateMergeRequestOptions options, string defaultProject)
        {
            var project = GetProject(options, defaultProject);
            if (project.IsFailure)
                return Fail<CreateMergeRequestParameters>(project);

            return Ok(new CreateMergeRequestParameters(
                options.Title,
                options.Source,
                options.Destination,
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself
            });
        }

        public Result<ListMergesParameters> NegotiateListMergeRequestsParameters(ListMergesOptions options, string defaultProject)
        {
            var project = GetProject(options, defaultProject);
            if (project.IsFailure)
                return Fail<ListMergesParameters>(project);

            var state = ParseMergeRequestState(options.State);
            if (!state.HasValue)
                return Fail<ListMergesParameters>($"State parameter: {options.State} is not supported." +
                                                          "Supported values are: opened|closed|merged");

            return Ok(new ListMergesParameters(
                project.Value,
                state.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignedToMe
            });
        }

        private static MergeRequestState? ParseMergeRequestState(string state)
        {
            switch (state.NormalizeSpaces().ToUpperInvariant())
            {
                case "":
                    return MergeRequestState.Opened;
                case "OPENED":
                case "OPEN":
                    return MergeRequestState.Opened;
                case "CLOSED":
                    return MergeRequestState.Closed;
                case "MERGED":
                    return MergeRequestState.Merged;
                case "ALL":
                    return MergeRequestState.All;
                default:
                    return null;
            }
        }
    }
}
