using System;

namespace GitlabCmd.Console.App
{
    public class OutputPresenter
    {
        public void Info(string text) 
            => System.Console.WriteLine(text);

        public void Error(string text)
            => System.Console.WriteLine($"Error: {text}");

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

        private void WriteLine(string text)
            => System.Console.WriteLine(text);
    }
}
