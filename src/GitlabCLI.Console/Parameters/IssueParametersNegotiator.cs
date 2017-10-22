using System.Collections.Generic;
using System.Linq;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Utilities;
using static GitLabCLI.Core.Result;

namespace GitLabCLI.Console.Parameters
{
    public class IssueParametersNegotiator : ProjectParametersNegotiator
    {
        public Result<BrowseParameters> NegotiateBrowseParameters(BrowseOptions options, string defaultProject)
        {
            var project = GetProject(options, defaultProject);
            if (project.IsFailure)
                return Fail<BrowseParameters>(project);

            return Ok(new BrowseParameters(project.Value, options.Id));
        }

        public Result<CreateIssueParameters> NegotiateCreateIssueParameters(
            CreateIssueOptions options, 
            string defaultProject, 
            string defaultIssuesLabel)
        {
            var project = GetProject(options, defaultProject);
            if (project.IsFailure)
                return Fail<CreateIssueParameters>(project);

            return Ok(new CreateIssueParameters(
                options.Title,
                options.Description,
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignMyself,
                Labels = GetLabels(options.Labels, defaultIssuesLabel)
            });
        }

        public Result<CloseIssueParameters> NegotiateCloseIssueParameters(CloseIssueOptions options, string defaultProject)
        {
            var project = GetProject(options, defaultProject);
            if (project.IsFailure)
                return Fail<CloseIssueParameters>(project);

            return Ok(new CloseIssueParameters(project.Value, options.Id));
        }

        public Result<ListIssuesParameters> NegotiateListIssuesParameters(ListIssuesOptions options, string defaultProject, string defaultIssuesLabel)
        {
            var project = GetProject(options, defaultProject);
            if (project.IsFailure)
                return Fail<ListIssuesParameters>(project);

            var outputFormat = ParseOutputFormat(options.Output);
            if (outputFormat.IsFailure)
                return Fail<ListIssuesParameters>(outputFormat);

            var issueState = ParseIssueState(options.State);
            if (!issueState.HasValue)
                return Fail<ListIssuesParameters>($"State parameter: {options.State} is not supported." +
                                                         "Supported values are: opened|closed|all");

            return Ok(new ListIssuesParameters(
                project.Value,
                options.Assignee)
            {
                AssignedToCurrentUser = options.AssignedToMe,
                Output = outputFormat.Value,
                Filter = options.Filter,
                IssuesIds = options.Ids.SafeToList(),
                Labels = GetLabels(options.Labels, defaultIssuesLabel)
            });
        }

        private List<string> GetLabels(IEnumerable<string> labels, string defaultIssuesLabel)
        {
            var inputLabels = labels.
                SafeToList().
                Where(l => !l.IsNullOrEmpty()).
                ToList();

            if (inputLabels.Any())
                return inputLabels;

            string normalizedDefaultLabel = defaultIssuesLabel.NormalizeSpaces();
            if (!normalizedDefaultLabel.IsNullOrEmpty())
                return new List<string> { normalizedDefaultLabel };

            return new List<string>();
        }

        private static IssueState? ParseIssueState(string state)
        {
            switch (state.NormalizeSpaces().ToUpperInvariant())
            {
                case "":
                    return IssueState.Opened;
                case "OPENED":
                case "OPEN":
                    return IssueState.Opened;
                case "CLOSED":
                    return IssueState.Closed;
                case "ALL":
                    return IssueState.All;
                default:
                    return null;
            }
        }

        private static Result<OutputFormat> ParseOutputFormat(string format)
        {
            if (format.NormalizeSpaces().EqualsIgnoringCase("rows") ||
                format.NormalizeSpaces().EqualsIgnoringCase(""))
            {
                return Ok(OutputFormat.Rows);
            }
            else if (format.NormalizeSpaces().EqualsIgnoringCase("grid"))
            {
                return Ok(OutputFormat.Grid);
            }
            else
            {
                return Fail<OutputFormat>(
                    $"Output format {format} is not recognized. " +
                    "Available values are: rows, grid. Default is rows.");
            }
        }
    }
}
