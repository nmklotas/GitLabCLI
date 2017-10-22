using System.Collections.Generic;
using FluentAssertions;
using GitLabCLI.Core;

namespace GitLabCLI.Console.Test.Common
{
    public class FakeConsoleWriter : IConsoleWriter
    {
        private readonly List<string> _messages = new List<string>();

        public void Write(string text) 
            => _messages.Add(text);

        public void ShouldHaveWritten(string text) 
            => _messages.Should().Contain(text);

        public void ShouldHaveWrittenError(string errorMessage)
            => _messages.Should().Contain($"Error: {errorMessage}");
    }
}
