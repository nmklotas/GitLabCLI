namespace GitlabCmd.Console.App
{
    public class OutputPresenter
    {
        public void Info(string text) 
            => System.Console.WriteLine(text);

        public void Error(string text)
            => System.Console.WriteLine($"Error: {text}");
    }
}
