using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GitLabCLI.Console.Configuration;
using GitLabCLI.Console.Parsing;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab.Issues;
using GitLabCLI.Core.Gitlab.Merges;
using Xunit;

namespace GitLabCLI.Console.Test.Output
{
    public sealed class ParametersHandlerTest
    {
        private readonly ParametersHandler _sut = new ParametersHandler(new AppSettings
        {
            DefaultProject = "appsettings-project",
            DefaulIssuesLabel = "appsettings-label",
            GitLabAccessToken = "appsettings-token",
            GitLabHostUrl = "https://test-gitlab.com"
        });

        [Fact]
        public void NotProvidedProjectTakenFromSettings()
        {
            string[] parameter =
            {
                _sut.NegotiateListIssuesParameters(new ListIssuesOptions()).Value.Project,
                _sut.NegotiateCreateIssueParameters(new CreateIssueOptions
                {
                    Title = "title"
                }).
                Value.Project,
                _sut.NegotiateListMergesParameters(new ListMergesOptions()).Value.Project,
                _sut.NegotiateCreateMergeRequestParameters(new CreateMergeRequestOptions
                {
                    Title = "title",
                    Source = "source",
                    Destination = "destination"
                }).Value.Project,
                _sut.NegotiateCloseIssueParameters(new CloseIssueOptions()).Value.Project
            };

            parameter.Should().OnlyContain(p => p == "appsettings-project");
        }

        [Fact]
        public void NotProvidedLabelsTakenFromSettings()
        {
            var parameter = new List<IList<string>>
            {
                _sut.NegotiateListIssuesParameters(new ListIssuesOptions()).Value.Labels,
                _sut.NegotiateCreateIssueParameters(new CreateIssueOptions
                {
                    Title = "title"
                }).
                Value.Labels,
            };

            parameter.Should().OnlyContain(
                p => p.SequenceEqual(new[] { "appsettings-label" }));
        }

        [Fact]
        public void CreateMergeRequestParametersNegotiated()
        {
            _sut.NegotiateCreateMergeRequestParameters(new CreateMergeRequestOptions
            {
                Title = "title",
                Source = "source",
                Destination = "destination",
                Assignee = "assignee",
                Project = "project",
                AssignMyself = true
            }).Value.Should().Match<CreateMergeRequestParameters>(s =>
                s.Title == "title" &&
                s.SourceBranch == "source" &&
                s.TargetBranch == "destination" &&
                s.Assignee == "assignee" &&
                s.Project == "project" &&
                s.AssignedToCurrentUser);
        }

        [Fact]
        public void CreateIssuesParametersNegotiated()
        {
            _sut.NegotiateCreateIssueParameters(new CreateIssueOptions
            {
                Title = "title",
                Description = "description",
                Assignee = "assignee",
                Labels = new[] { "label" },
                Project = "project",
                AssignMyself = true
            }).Value.Should().Match<CreateIssueParameters>(s =>
                s.Title == "title" &&
                s.Description == "description" &&
                s.Assignee == "assignee" &&
                s.Labels.SequenceEqual(new[] { "label" }) &&
                s.Project == "project" &&
                s.AssignedToCurrentUser);
        }

        [Fact]
        public void CloseIssuesParametersNegotiated()
        {
            _sut.NegotiateCloseIssueParameters(new CloseIssueOptions
            {
                Id = 1,
                Project = "project"
            }).Value.Should().Match<CloseIssueParameters>(s =>
                s.Project == "project" &&
                s.IssueId == 1);
        }

        [Fact]
        public void BrowseIssueParametersNegotiated()
        {
            _sut.NegotiateBrowseParameters(new BrowseOptions()
            {
                Id = 1,
                Project = "project"
            }).Value.Should().Match<BrowseParameters>(s =>
                s.Project == "project" &&
                s.IssueId == 1);
        }

        [Fact]
        public void ListIssuesParametersNegotiated()
        {
            _sut.NegotiateListIssuesParameters(new ListIssuesOptions
            {
                AssignedToMe = true,
                Assignee = "assignee",
                Labels = new[] { "label" },
                Output = "grid",
                Filter = "filter",
                State = "opened",
                Project = "project",
                Ids = new[] { 1, 2 },
            }).Value.Should().Match<ListIssuesParameters>(s =>
                s.AssignedToCurrentUser &&
                s.Assignee == "assignee" &&
                s.Output == OutputFormat.Grid &&
                s.Filter == "filter" &&
                s.Labels.SequenceEqual(new[] { "label" }) &&
                s.IssueState == IssueState.Opened &&
                s.Project == "project" &&
                s.IssuesIds.SequenceEqual(new[] { 1, 2 }));
        }

        [Fact]
        public void ListMergesParametersNegotiated()
        {
            _sut.NegotiateListMergesParameters(new ListMergesOptions
            {
                AssignedToMe = true,
                Assignee = "assignee",
                State = "merged",
                Project = "project"
            }).Value.Should().Match<ListMergesParameters>(s =>
                s.AssignedToCurrentUser &&
                s.Assignee == "assignee" &&
                s.State == MergeRequestState.Merged &&
                s.Project == "project");
        }
    }
}
