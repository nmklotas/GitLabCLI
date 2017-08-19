using System.Collections.Generic;
using System.Linq;
using CommandLine;
using FluentAssertions;
using GitlabCmd.Console.Cmd;
using Xunit;

namespace GitlabCmd.Console.Test.Cmd
{
    public class LaunchHandlerParsingTest
    {
        private readonly ParserSpy _sut = new ParserSpy(Parser.Default);

        [Theory]
        [InlineData(
            "create", "issue",
            "-t", "test title",
            "-d", "test description",
            "-a", "testUser",
            "-l", "testlabel1,testlabel2",
            "-p", "testProject", 
            "--assign-myself")]
        [InlineData(
            "create", "issue",
            "--title", "test title",
            "--description", "test description",
            "--assignee", "testUser",
            "--labels", "testlabel1,testlabel2",
            "--project", "testProject",
            "--assign-myself")]
        public void CreateIssueWithShortArgsParsedAsCreateIssueOptions(params string[] args)
        {
            //act
            _sut.Parse(args);

            //assert
            _sut.Options.Should().Match<CreateIssueOptions>(
                s => s.Title == "test title" && 
                s.Description == "test description" && 
                s.Assignee == "testUser" &&
                s.AssignMyself &&
                s.Labels.SequenceEqual(new[] { "testlabel1", "testlabel2"}) &&
                s.Project == "testProject");
        }

        [Theory]
        [InlineData(
            "create", "merge",
            "-a", "testUser",
            "-t", "testtitle",
            "-s", "testSourceBranch",
            "-d", "testDestinationBranch",
            "-p", "testProject")]
        [InlineData(
            "create", "merge",
            "--assignee", "testUser",
            "--title", "testtitle",
            "--source", "testSourceBranch",
            "--destination", "testDestinationBranch",
            "--project", "testProject")]
        public void AddMergeCallsCreateIsuesWithDescriptionHavingSpaces(params string[] args)
        {
            //act
            _sut.Parse(args);

            //assert
            _sut.Options.Should().Match<CreateMergeRequestOptions>(
                s => s
                .Title == "testtitle" && 
                s.Destination == "testDestinationBranch" &&
                s.Source == "testSourceBranch" && 
                s.Assignee == "testUser" &&
                s.Project == "testProject");
        }

        [Theory]
        [InlineData(
            "issue", "ls",
            "--assigned-to-me",
            "-l", "testlabel1")]
        [InlineData(
            "issue", "ls",
            "--assigned-to-me",
            "--labels", "testlabel1")]
        public void ListIssuesParsedAsListIssueOptions(params string[] args)
        {
            //act
            _sut.Parse(args);

            //assert
            _sut.Options.Should().Match<ListIssuesOptions>(
                s => s.AssignedToMe &&
                     s.Labels.SequenceEqual(new[] { "testlabel1" }));
        }

        private class ParserSpy
        {
            private readonly Parser _parser;

            public ParserSpy(Parser parser) => _parser = parser;

            public void Parse(string[] args) => _parser.
                ParseVerbs<
                    CreateOptions,
                    IssueOptions,
                    GitlabCmdConfigurationOptions>(args).
                MapResult(
                    (CreateOptions options) => Options = options,
                    (IssueOptions options) => Options = options,
                    (GitlabCmdConfigurationOptions options) => Options = options,
                    errors => Errors = errors);

            public object Options { get; set; }

            public IEnumerable<Error> Errors { get; set; }
        }
    }
}
