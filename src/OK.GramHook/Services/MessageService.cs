using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OK.GramHook.Services
{
    internal class MessageService : IMessageService
    {
        private const string SendMessageApiUrl = "https://api.telegram.org/bot{0}/sendMessage";
        private const string SendMessageApiContentType = "application/json";

        private readonly string _botToken;

        public MessageService(string botToken)
        {
            _botToken = botToken;
        }

        public async Task SendAsync(string chatId, string message)
        {
            string json = JsonConvert.SerializeObject(new
            {
                chat_id = chatId,
                text = message
            });

            string url = string.Format(SendMessageApiUrl, _botToken);

            HttpContent content = new StringContent(json, Encoding.UTF8, SendMessageApiContentType);

            await new HttpClient().PostAsync(url, content);
        }
    }
}