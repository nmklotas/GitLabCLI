using FluentAssertions;
using GitlabCmd.Console.App;
using Xunit;

namespace GitlabCmd.Console.Test.App
{
    public class GridResultFormatterTest
    {
        [Fact]
        public void GridIsFormatted()
        {
            var sut = new GridResultFormatter();
            string result = sut.Format(
                "TestHeader",
                new[] {"test10", "test20", "test300"},
                new object[][]
                {
                   new[] { "test1", "test2", "test3" },
                   new[] { "test1111", "test222", "test333" }
                });


            string[] lines = result.Split("\r\n");
            lines.Should().HaveCount(7);
            lines[0].Should().Be("-------------------------");
            lines[1].Should().Be("TestHeader");
            lines[2].Should().BeEmpty();
            lines[3].Should().Be("test10    test20   test300");
            lines[4].Should().Be("________  _______  _______");
            lines[5].Should().Be("test1     test2    test3  ");
            lines[6].Should().Be("test1111  test222  test333");
        }
    }
}
