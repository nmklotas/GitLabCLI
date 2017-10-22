using FluentAssertions;
using GitLabCLI.Console.Parameters;
using GitLabCLI.Console.Parsing;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace GitLabCLI.Console.Test.Parameters
{
    public class ProjectParametersNegotiatorTest : ProjectParametersNegotiator
    {
        [AutoData]
        [Theory]
        public void ProjectIsTakenFromOptions(ProjectOptionsStub options, string defaultProject)
        {
            GetProject(options, defaultProject).Value.Should().Be(options.Project);
        }

        [AutoData]
        [Theory]
        public void NotSetProjectIsTakenFromDefaultProject(ProjectOptionsStub options, string defaultProject)
        {
            options.Project = "";
            GetProject(options, defaultProject).Value.Should().Be(defaultProject);
        }

        [AutoData]
        [Theory]
        public void ProjectAndDefaultProjectMissingResultsInFailure(ProjectOptionsStub options)
        {
            options.Project = "";
            GetProject(options, "").IsFailure.Should().BeTrue();
        }

        public class ProjectOptionsStub : ProjectOptions
        {
        }
    }
}
