using System.Linq;
using FluentAssertions;
using GitLabCLI.Console.Parameters;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Issues;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace GitLabCLI.Console.Test.Parameters
{
    public class IssuesParametersNegotiatorTest
    {
        [AutoData]
        [Theory]
        public void NegotiateCloseIssueParametersCreatesParametersFromOptions(CloseIssueOptions options, string defaultProject)
        {
            var sut = new IssueParametersNegotiator();
            sut.NegotiateCloseIssueParameters(options, defaultProject).Value.Should().Match<CloseIssueParameters>(o =>
                o.IssueId == options.Id &&
                o.Project == options.Project);
        }

        [AutoData]
        [Theory]
        public void NegotiateCloseIssueParametersReturnsFailureForMissingProject(CloseIssueOptions options)
        {
            options.Project = "";
            var sut = new IssueParametersNegotiator();
            sut.NegotiateCloseIssueParameters(options, "").IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateBrowseIssueParametersCreatesParametersFromOptions(BrowseOptions options, string defaultProject)
        {
            var sut = new IssueParametersNegotiator();
            sut.NegotiateBrowseParameters(options, defaultProject).Value.Should().Match<BrowseParameters>(o =>
                o.IssueId == options.Id &&
                o.Project == options.Project);
        }

        [AutoData]
        [Theory]
        public void NegotiateBrowseIssueParametersReturnsFailureForMissingProject(BrowseOptions options)
        {
            options.Project = "";
            var sut = new IssueParametersNegotiator();
            sut.NegotiateBrowseParameters(options, "").IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateCreateIssueParametersReturnsFailureForMissingProject(CreateIssueOptions options, string defaultIssuesLabel)
        {
            options.Project = "";
            var sut = new IssueParametersNegotiator();
            sut.NegotiateCreateIssueParameters(options, "", defaultIssuesLabel).IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateCreateIssueParametersCreatesParametersFromOptions(CreateIssueOptions options, string defaultProject, string defaultIssuesLabel)
        {
            var sut = new IssueParametersNegotiator();
            sut.NegotiateCreateIssueParameters(options, defaultProject, defaultIssuesLabel).Value.
                Should().
                Match<CreateIssueParameters>(o => 
                    o.Title == options.Title &&
                    o.Description == options.Description &&
                    o.Project == options.Project &&
                    o.Assignee == options.Assignee &&
                    o.AssignedToCurrentUser == options.AssignMyself &&
                    o.Labels.SequenceEqual(options.Labels));
        }

        [AutoData]
        [Theory]
        public void NegotiateCreateIssueParametersTakesLabelsFromOptions(CreateIssueOptions options, string defaultProject, string defaultIssuesLabel)
        {
            var sut = new IssueParametersNegotiator();
            sut.NegotiateCreateIssueParameters(options, defaultProject, defaultIssuesLabel).
                Value.Labels.
                Should().
                BeEquivalentTo(options.Labels);
        }

        [AutoData]
        [Theory]
        public void NegotiateCreateIssueParametersTakesNotProvidedLabelsFromDefault(
            CreateIssueOptions options,
            string defaultProject,
            string defaultIssuesLabel)
        {
            options.Labels = new string[] { };

            var sut = new IssueParametersNegotiator();

            sut.NegotiateCreateIssueParameters(options, defaultProject, defaultIssuesLabel).
                Value.Labels.
                Should().
                BeEquivalentTo(defaultIssuesLabel);
        }

        [AutoData]
        [Theory]
        public void NegotiateListIssuesParametersReturnsFailureForMissingProject(ListIssuesOptions options, string defaultIssuesLabel)
        {
            options.Project = "";
            var sut = new IssueParametersNegotiator();
            sut.NegotiateListIssuesParameters(options, "", defaultIssuesLabel).IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateListIssuesParametersReturnsFailureForUnknownIssueState(
            ListIssuesOptions options, 
            string defaultProject,
            string defaultIssuesLabel,
            string unknownIssueState)
        {
            options.State = unknownIssueState;
            var sut = new IssueParametersNegotiator();
            sut.NegotiateListIssuesParameters(options, defaultProject, defaultIssuesLabel).IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateListIssuesParametersReturnsFailureForUnknownOutputType(
            ListIssuesOptions options,
            string defaultProject,
            string defaultIssuesLabel,
            string unknownOutputType)
        {
            options.Output = unknownOutputType;
            var sut = new IssueParametersNegotiator();
            sut.NegotiateListIssuesParameters(options, defaultProject, defaultIssuesLabel).IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateListIssuesParametersCreatesParametersFromOptions(
            ListIssuesOptions options,
            string defaultProject,
            string defaultIssuesLabel)
        {
            options.Output = "rows";
            options.State = "opened";
            var sut = new IssueParametersNegotiator();
            sut.NegotiateListIssuesParameters(options, defaultProject, defaultIssuesLabel).Value.
                Should().
                Match<ListIssuesParameters>(o => 
                    o.Project == options.Project &&
                    o.Assignee == options.Assignee &&
                    o.Filter == options.Filter &&
                    o.Output == OutputFormat.Rows &&
                    o.AssignedToCurrentUser == options.AssignedToMe &&
                    o.IssueState == IssueState.Opened &&
                    o.IssuesIds.SequenceEqual(options.Ids) &&
                    o.Labels.SequenceEqual(options.Labels));
        }

        [AutoData]
        [Theory]
        public void NegotiateListIssueParametersTakesLabelsFromOptions(
            ListIssuesOptions options, 
            string defaultProject,
            string defaultIssuesLabel)
        {
            options.Output = "rows";
            options.State = "opened";
            var sut = new IssueParametersNegotiator();
            sut.NegotiateListIssuesParameters(options, defaultProject, defaultIssuesLabel).
                Value.Labels.
                Should().
                BeEquivalentTo(options.Labels);
        }

        [AutoData]
        [Theory]
        public void NegotiateListIssueParametersTakesNotProvidedLabelsFromDefault(
            ListIssuesOptions options,
            string defaultProject,
            string defaultIssuesLabel)
        {
            options.Output = "rows";
            options.State = "opened";
            options.Labels = new string[] { };

            var sut = new IssueParametersNegotiator();

            sut.NegotiateListIssuesParameters(options, defaultProject, defaultIssuesLabel).
                Value.Labels.
                Should().
                BeEquivalentTo(defaultIssuesLabel);
        }
    }
}
