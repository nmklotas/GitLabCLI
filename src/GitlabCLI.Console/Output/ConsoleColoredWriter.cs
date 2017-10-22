using System;
using GitLabCLI.Core;

namespace GitLabCLI.Console.Output
{
    public class ConsoleColoredWriter : IConsoleWriter
    {
        public void Write(string text)
        {
            foreach (string line in text.Split(Environment.NewLine))
                WriteColoredLine(line);
        }

        private static void WriteColoredLine(string line)
        {
            var parsedLine = ParseColoredLine(line);
            System.Console.ForegroundColor = parsedLine.Color;
            System.Console.WriteLine(parsedLine.Text);
            System.Console.ResetColor();
        }

        private static (ConsoleColor Color, string Text) ParseColoredLine(string line)
        {
            if (line.StartsWith(ConsoleOutputColor.Yellow, StringComparison.OrdinalIgnoreCase))
                return (ConsoleColor.DarkYellow, line.Replace(ConsoleOutputColor.Yellow, ""));

            return (ConsoleColor.White, line);
        }
    }
}
