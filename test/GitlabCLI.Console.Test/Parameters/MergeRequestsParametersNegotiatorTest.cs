using FluentAssertions;
using GitLabCLI.Console.Parameters;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core.Gitlab.Merges;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace GitLabCLI.Console.Test.Parameters
{
    public class MergeRequestsParametersNegotiatorTest
    {
        [AutoData]
        [Theory]
        public void NegotiateCreateMergeRequestParametersCreatesParametersFromOptions(CreateMergeRequestOptions options, string defaultProject)
        {
            var sut = new MergeRequestsParametersNegotiator();

            sut.NegotiateCreateMergeRequestParameters(options, defaultProject).Value.Should().Match<CreateMergeRequestParameters>(o =>
                o.Title == options.Title &&
                o.SourceBranch == options.Source &&
                o.TargetBranch == options.Destination &&
                o.Assignee == options.Assignee &&
                o.AssignedToCurrentUser == options.AssignMyself &&
                o.Project == options.Project);
        }

        [AutoData]
        [Theory]
        public void NegotiateListMergeRequestParametersCreatesParametersFromOptions(ListMergesOptions options, string defaultProject)
        {
            options.State = "opened";

            var sut = new MergeRequestsParametersNegotiator();

            sut.NegotiateListMergeRequestsParameters(options, defaultProject).Value.Should().Match<ListMergesParameters>(o =>
                o.State == MergeRequestState.Opened &&
                o.Assignee == options.Assignee &&
                o.AssignedToCurrentUser == options.AssignedToMe &&
                o.Project == options.Project);
        }

        [AutoData]
        [Theory]
        public void NegotiateListMergeRequestParametersReturnsFailureForUnknownState(
            ListMergesOptions options, 
            string defaultProject, 
            string unknownState)
        {
            options.State = unknownState;

            var sut = new MergeRequestsParametersNegotiator();

            sut.NegotiateListMergeRequestsParameters(options, defaultProject).IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateListMergeRequestsParametersReturnsFailureForMissingProject(CreateMergeRequestOptions options)
        {
            options.Project = "";
            var sut = new MergeRequestsParametersNegotiator();
            sut.NegotiateCreateMergeRequestParameters(options, "").IsFailure.Should().BeTrue();
        }

        [AutoData]
        [Theory]
        public void NegotiateCreateMergeRequestParametersReturnsFailureForMissingProject(CreateMergeRequestOptions options)
        {
            options.Project = "";
            var sut = new MergeRequestsParametersNegotiator();
            sut.NegotiateCreateMergeRequestParameters(options, "").IsFailure.Should().BeTrue();
        }
    }
}
