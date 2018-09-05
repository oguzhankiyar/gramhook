using System;

namespace OK.GramHook
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CommandCaseAttribute : Attribute
    {
        public string[] Arguments { get; set; }

        public CommandCaseAttribute(params string[] args)
        {
            Arguments = args;
        }
    }
}