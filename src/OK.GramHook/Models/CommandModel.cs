using System;
using System.Collections.Generic;
using System.Reflection;

namespace OK.GramHook.Models
{
    internal class CommandModel
    {
        public List<ArgumentModel> ArgumentDetails { get; set; }

        public MethodInfo Method { get; set; }

        public Type Type { get; set; }
    }
}