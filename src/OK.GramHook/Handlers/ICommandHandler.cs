using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OK.GramHook.Handlers
{
    internal interface ICommandHandler
    {
        Task HandleAsync(HttpContext context);
    }
}