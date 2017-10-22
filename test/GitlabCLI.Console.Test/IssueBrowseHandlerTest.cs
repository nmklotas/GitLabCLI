using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabCLI.Console.Output;
using GitLabCLI.Console.Test.Common;
using GitLabCLI.Core;
using GitLabCLI.Core.Gitlab;
using GitLabCLI.Core.Gitlab.Issues;
using NSubstitute;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace GitLabCLI.Console.Test
{
    public class IssueBrowseHandlerTest
    {
        [Theory]
        [AutoData]
        public async Task BrowseNavigatesToRequestedIssue(string project, Issue[] issues)
        {
            //arrange
            var facadeStub = Substitute.For<IGitLabFacade>();
            facadeStub.ListIssues(Arg.Any<ListIssuesParameters>()).Returns(Result.Ok<IReadOnlyList<Issue>>(issues));

            var browserMock = Substitute.For<IBrowser>();
            var sut = CreateSut(facadeStub, browserMock, new FakeConsoleWriter());

            //act
            var requestedIssue = issues.First();
            await sut.Browse(new BrowseParameters(project, requestedIssue.Id));

            //assert
            browserMock.Received().Open(requestedIssue.WebUrl);
        }

        [Theory]
        [AutoData]
        public async Task BrowseWritesErrorIfIssuesRequestFailed(string project, string error, Issue issue)
        {
            //arrange
            var facadeStub = Substitute.For<IGitLabFacade>();
            facadeStub.ListIssues(Arg.Any<ListIssuesParameters>()).Returns(Result.Fail<IReadOnlyList<Issue>>(error));

            var browser = Substitute.For<IBrowser>();
            var fakeConsoleWriter = new FakeConsoleWriter();
            var sut = CreateSut(facadeStub, browser, fakeConsoleWriter);

            //act
            await sut.Browse(new BrowseParameters(project, issue.Id));

            //assert
            fakeConsoleWriter.ShouldHaveWrittenError(error);
        }

        [Theory]
        [AutoData]
        public async Task BrowseWritesErrorIfNoIssuesFound(string project, string error, Issue issue)
        {
            //arrange
            var facadeStub = Substitute.For<IGitLabFacade>();
            facadeStub.ListIssues(Arg.Any<ListIssuesParameters>()).Returns(Result.Ok<IReadOnlyList<Issue>>(new Issue[] { }));

            var browser = Substitute.For<IBrowser>();
            var fakeConsoleWriter = new FakeConsoleWriter();
            var sut = CreateSut(facadeStub, browser, fakeConsoleWriter);

            //act
            await sut.Browse(new BrowseParameters(project, issue.Id));

            //assert
            fakeConsoleWriter.ShouldHaveWritten($"Issue #{issue.Id} was not found in project {project}");
        }

        private static IssueBrowseHandler CreateSut(IGitLabFacade facade, IBrowser browser, IConsoleWriter consoleWriter)
        {
            return new IssueBrowseHandler(facade, browser, new OutputPresenter(
                new GridResultFormatter(),
                new RowResultFormatter(),
                consoleWriter));
        }
    }
}
