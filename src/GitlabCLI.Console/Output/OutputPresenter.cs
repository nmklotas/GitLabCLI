namespace GitLabCLI.Console.Output
{
    public sealed class OutputPresenter
    {
        private readonly GridResultFormatter _gridResultFormatter;

        public OutputPresenter(GridResultFormatter gridResultFormatter) 
            => _gridResultFormatter = gridResultFormatter;

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
            params GridRow[] rows)
        {
            WriteLine(_gridResultFormatter.Format(header, rows));
        }

        private void WriteLine(string text)
            => System.Console.WriteLine(text);
    }
}
