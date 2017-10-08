using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GitLabCLI.Console.Output;
using GitLabCLI.Console.Parsing;

namespace GitLabCLI.Console
{
    public sealed class LaunchHandler
    {
        private readonly CommandLineArgsParser _parser;
        private readonly LaunchOptionsVisitor _optionsVisitor;
        private readonly OutputPresenter _outputPresenter;

        public LaunchHandler(
            CommandLineArgsParser parser, 
            LaunchOptionsVisitor optionsVisitor,
            OutputPresenter outputPresenter)
        {
            _parser = parser;
            _optionsVisitor = optionsVisitor;
            _outputPresenter = outputPresenter;
        }

        public async Task<int> Launch(string[] args)
        {
            try
            {
                IVisitableOption visitable = _parser.Parse(args);
                if (visitable == null)
                    return ExitCode.InvalidArguments;

                await visitable.Accept(_optionsVisitor);
            }
            catch (Exception ex)
            {
                _outputPresenter.FailureResult("Unexpected error has occured", ex.ToString());
                return ExitCode.UnexpectedFailure;
            }
            finally
            {
                WaitForInput();
            }

            return ExitCode.Success;
        }

        [Conditional("DEBUG")]
        private static void WaitForInput() =>
            System.Console.ReadLine();
    }
}
