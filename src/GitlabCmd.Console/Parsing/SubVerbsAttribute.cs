using System;

namespace GitLabCmd.Console.Parsing
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SubVerbsAttribute : Attribute
    {
        public Type[] Types { get; }

        public SubVerbsAttribute(params Type[] types) => Types = types;
    }
}