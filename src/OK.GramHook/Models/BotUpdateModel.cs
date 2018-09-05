using Newtonsoft.Json;

namespace OK.GramHook.Models
{
    internal class BotUserModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    internal class BotChatModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }

    internal class BotMessageModel
    {
        [JsonProperty("message_id")]
        public int Id { get; set; }

        [JsonProperty("from")]
        public BotUserModel From { get; set; }

        [JsonProperty("chat")]
        public BotChatModel Chat { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    internal class BotUpdateModel
    {
        [JsonProperty("update_id")]
        public int Id { get; set; }

        [JsonProperty("message")]
        public BotMessageModel Message { get; set; }
    }
}