using System.Threading.Tasks;

namespace OK.GramHook.Services
{
    internal interface IMessageService
    {
        Task SendAsync(string chatId, string message);
    }
}