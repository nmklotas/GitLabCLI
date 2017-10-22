using GitLabCLI.Core;

namespace GitLabCLI.Console.Output
{
    public sealed class OutputPresenter
    {
        private readonly GridResultFormatter _gridResultFormatter;
        private readonly RowResultFormatter _rowResultFormatter;
        private readonly IConsoleWriter _coloredWriter;

        public OutputPresenter(
            GridResultFormatter gridResultFormatter, 
            RowResultFormatter rowResultFormatter,
            IConsoleWriter coloredWriter)
        {
            _gridResultFormatter = gridResultFormatter;
            _rowResultFormatter = rowResultFormatter;
            _coloredWriter = coloredWriter;
        }

        public void ShowMessage(string text)
            => WriteLine(text);

        public void ShowError(string text)
            => WriteLine($"Error: {text}");

        public void ShowSuccess(string header)
        {
            WriteLine("-------------------------");
            WriteLine(header);
        }

        public void ShowError(string header, string error)
        {
            WriteLine("-------------------------");
            WriteLine(header);
            WriteLine($"Error: {error}");
        }

        public void ShowGrid(string header, params GridColumn[] columns) 
            => WriteLine(_gridResultFormatter.Format(header, columns));

        public void ShowRows(string header, params Row[] rows)
            => WriteLine(_rowResultFormatter.Format(header, rows));

        private void WriteLine(string text)
            => _coloredWriter.Write(text);
    }
}
