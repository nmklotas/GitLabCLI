using System;
using System.Threading.Tasks;
using FluentAssertions;
using GitlabCmd.Console.GitLab;
using NGitLab;
using Xunit;
using static GitlabCmd.Console.Test.GitLab.GitLabApiHelper;

namespace GitlabCmd.Console.Test.GitLab
{
    public class GitLabFacadeTest
    {
        private readonly GitLabFacade _sut = new GitLabFacade(
            new Lazy<GitLabClient>(Client));

        [Fact]
        public async Task AddIssueCreatesIssue()
        {
            var addIssueResult = await _sut.AddIssue(
                "title1", "description1", 
                ProjectName, 
                UserName,
                new[] {"label1", "label2"});

            addIssueResult.IsSuccess.Should().BeTrue();

            await ShouldHaveIssue(
                ProjectName, 
                addIssueResult.Value, issue =>
                {
                    issue.Title.Should().Be("title1");
                    issue.Description.Should().Be("description1");
                    issue.Assignee.Username.Should().Be(UserName);
                    issue.Labels.Should().BeEquivalentTo("label1", "label2");
                    issue.State.Should().BeEquivalentTo("Open");
                });
        }
    }
}
