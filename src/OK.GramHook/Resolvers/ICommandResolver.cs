using OK.GramHook.Models;
using System.Reflection;

namespace OK.GramHook.Builders
{
    internal interface ICommandResolver
    {
        void Register(Assembly assembly);

        ResolveModel Resolve(string text);
    }
}