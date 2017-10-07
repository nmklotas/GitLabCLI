using System;
using System.Collections.Generic;
using System.Linq;

namespace GitLabCLI.Console.Output
{
    public sealed class OutputPresenter
    {
        private readonly GridResultFormatter _gridResultFormatter;

        public OutputPresenter(GridResultFormatter gridResultFormatter) => _gridResultFormatter = gridResultFormatter;

        public void Info(string text)
            => WriteLine(text);

        public void Error(string text)
            => WriteLine($"Error: {text}");

        public void SuccessResult(string header)
        {
            WriteLine("-------------------------");
            WriteLine(header);
        }

        public void FailureResult(string header, string error)
        {
            WriteLine("-------------------------");
            WriteLine(header);
            WriteLine($"Error: {error}");
        }

        public void GridResult(
            string header,
            string[] columnHeaders,
            IEnumerable<object[]> rows)
        {
            var inputRows = rows.ToArray();
            if (inputRows.Select(r => r.Length).Any(l => l != columnHeaders.Length))
                throw new ArgumentOutOfRangeException(nameof(columnHeaders), "columnHeaders length must match all rows length");

            WriteLine(_gridResultFormatter.Format(header, columnHeaders, inputRows));
        }

        private void WriteLine(string text)
            => System.Console.WriteLine(text);
    }
}
