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
        private readonly ParserSpy _sut = new ParserSpy(new Parser());

        [Theory]
        [InlineData(
            "add-issue", 
            "-t", "testtitle")]
        [InlineData(
            "add-issue", 
            "--title", "test title")]
        public void AddIssueCallsCreateIsuesWithTitle(params string[] args)
        {  
            //act
            _sut.Parse(args);

            //assert
            _sut.Options.
                Should().
                Match<AddIssueOptions>(
                    s => s.Title == args.Last());
        }

        [Theory]
        [InlineData(
            "add-issue", 
            "-t", "testtitle", 
            "-d", "testdescription")]
        public void AddIssueCallsCreateIsuesWithDescription(params string[] args)
        {
            //act
            _sut.Parse(args);

            //assert
            _sut.Options.
                Should().
                Match<AddIssueOptions>(
                    s => s.Title == "testtitle" && s.Description == "testdescription");
        }

        [Theory]
        [InlineData(
            "add-issue",
            "-t", "test title",
            "--description", "test description")]
        public void AddIssueCallsCreateIsuesWithDescriptionHavingSpaces(params string[] args)
        {
            //act
            _sut.Parse(args);

            //assert
            _sut.Options.Should().Match<AddIssueOptions>(
                s => s.Title == "test title" && s.Description == "test description");
        }

        private class ParserSpy
        {
            private readonly Parser _parser;

            public ParserSpy(Parser parser) => _parser = parser;

            public void Parse(string[] args) => _parser.
                ParseArguments<
                    AddIssueOptions,
                    GitlabCmdConfigurationOptions,
                    CreateMergeRequestOptions>(args).
                MapResult(
                    (AddIssueOptions options) => Options = options,
                    (GitlabCmdConfigurationOptions options) => Options = options,
                    (CreateMergeRequestOptions options) => Options = options,
                    errors => Errors = errors);

            public object Options { get; set; }

            public IEnumerable<Error> Errors { get; set; }
        }
    }
}
