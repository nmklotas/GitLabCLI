using FluentAssertions;
using GitLabCLI.Console.Output;
using Xunit;

namespace GitLabCLI.Console.Test.Output
{
    public sealed class GridResultFormatterTest
    {
        [Fact]
        public void GridIsFormatted()
        {
            string result = new GridResultFormatter().Format(
                "TestHeader",
                new GridRow("test10", 10, new object[] { "test1", "test1111" }),
                new GridRow("test20", 10, new object[] { "test2", "test222" }),
                new GridRow("test300", 10, new object[] { "test3", "test333" }));

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

