using System;
using System.Collections.Generic;
using System.Reflection;

namespace OK.GramHook.Models
{
    internal class ResolveModel
    {
        public Type CommandType { get; set; }

        public MethodInfo Method { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
    }
}