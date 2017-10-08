using FluentAssertions;
using GitLabCLI.Console.Output;
using Xunit;

namespace GitLabCLI.Console.Test.Output
{
    public class RowResultFormatterTest
    {
        [Fact]
        public void RowsAreFormatted()
        {
            var sut = new RowResultFormatter();
            string result = sut.Format(
                "TestHeader",
                new Row(new[] { "test10"}, new[] { "test10 value"}, "long test10 body description"),
                new Row(new[] { "test20" }, new[] { "test20 value" }, "long test20 body description"),
                new Row(new[] { "test300" }, new[] { "test300 value" }, "long test300 body description"));

            string[] lines = result.Split("\r\n");

            lines[0].Should().Be("-------------------------");
            lines[1].Should().Be("TestHeader");

            lines[2].Should().BeEmpty();
            lines[3].Should().Be("test10: test10 value");
            lines[4].Should().BeEmpty();
            lines[5].Should().Be("long test10 body description");

            lines[6].Should().BeEmpty();
            lines[7].Should().Be("test20: test20 value");
            lines[8].Should().BeEmpty();
            lines[9].Should().Be("long test20 body description");

            lines[10].Should().BeEmpty();
            lines[11].Should().Be("test300: test300 value");
            lines[12].Should().BeEmpty();
            lines[13].Should().Be("long test300 body description");

            lines.Should().HaveCount(14);
        }
    }
}
