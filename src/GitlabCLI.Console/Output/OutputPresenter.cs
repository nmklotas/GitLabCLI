namespace GitLabCLI.Console.Output
{
    public sealed class OutputPresenter
    {
        private readonly GridResultFormatter _gridResultFormatter;
        private readonly RowResultFormatter _rowResultFormatter;

        public OutputPresenter(GridResultFormatter gridResultFormatter, RowResultFormatter rowResultFormatter)
        {
            _gridResultFormatter = gridResultFormatter;
            _rowResultFormatter = rowResultFormatter;
        }

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

        public void GridResult(string header, params GridColumn[] columns) 
            => WriteLine(_gridResultFormatter.Format(header, columns));

        public void RowResult(string header, params Row[] rows)
            => WriteLine(_rowResultFormatter.Format(header, rows));
        
        private static void WriteLine(string text)
            => System.Console.WriteLine(text);
    }
}
