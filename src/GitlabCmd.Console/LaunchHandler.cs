using System.Threading.Tasks;
using GitlabCmd.Console.Parsing;

namespace GitlabCmd.Console
{
    public sealed class LaunchHandler
    {
        private readonly CommandLineArgsParser _parser;
        private readonly LaunchOptionsVisitor _optionsVisitor;

        public LaunchHandler(
            CommandLineArgsParser parser, 
            LaunchOptionsVisitor optionsVisitor)
        {
            _parser = parser;
            _optionsVisitor = optionsVisitor;
        }

        public async Task<int> Launch(string[] args)
        {
            IVisitableOption visitable = _parser.Parse(args);
            if (visitable == null)
                return ExitCode.InvalidArguments;

            await visitable.Accept(_optionsVisitor);
            return ExitCode.Success;
        }
    }
}
