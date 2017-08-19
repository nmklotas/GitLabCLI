using System;

namespace GitlabCmd.Console.Cmd
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SubVerbsAttribute : Attribute
    {
        public Type[] Types { get; }

        public SubVerbsAttribute(params Type[] types) => Types = types;
    }
}