using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace GitLabCmd.Console.Parsing
{
    public sealed class CommandLineArgsParser
    {
        private readonly Parser _parser;

        public CommandLineArgsParser(Parser parser) => _parser = parser;

        public IVisitableOption Parse(IEnumerable<string> args) => 
            (IVisitableOption)ParseVerbs(
                args,
                typeof(IssueOptions),
                typeof(MergeOptions),
                typeof(ConfigurationOptions),
                typeof(object)).MapResult<
                    IssueOptions, 
                    MergeOptions, 
                    ConfigurationOptions,
                    object>(
                        o => o,
                        o => o,
                        o => o,
                        e => null);

        private ParserResult<object> ParseVerbs(IEnumerable<string> args, params Type[] types)
        {
            var argsArray = args.ToArray();
            if (argsArray.Length == 0 || argsArray[0].StartsWith("-"))
                return _parser.ParseArguments(argsArray, types);

            string verb = argsArray[0];

            foreach (var type in types)
            {
                var verbAttribute = type.GetCustomAttribute<VerbAttribute>();
                if (verbAttribute == null || verbAttribute.Name != verb)
                    continue;

                var subVerbsAttribute = type.GetCustomAttribute<SubVerbsAttribute>();
                if (subVerbsAttribute != null)
                    return ParseVerbs(argsArray.Skip(1).ToArray(), subVerbsAttribute.Types);

                break;
            }

            return _parser.ParseArguments(argsArray, types);
        }
    }
}
