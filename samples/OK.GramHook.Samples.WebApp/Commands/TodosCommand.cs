using OK.GramHook;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OK.GramHook.Samples.WebApp.Commands
{
    // todos
    // tds
    [Command("todos|tds")]
    public class TodosCommand : CommandBase
    {
        private static IDictionary<string, IDictionary<string, string>> UserTodoItems = new Dictionary<string, IDictionary<string, string>>();

        public override async Task OnPreExecutionAsync()
        {
            await ReplyAsync("On Pre Execution");

            if (!UserTodoItems.ContainsKey(Context.ChatId) && !Context.MessageText.Contains("auth"))
            {
                await ReplyAsync("You are not authorized!\nTo authenticate, type 'todos auth app_pass'.");

                await AbortAsync();
            }
        }

        public override async Task OnPostExecutionAsync()
        {
            await ReplyAsync("On Post Execution");
        }

        // todos
        // todos help
        // todos h
        [CommandCase]
        [CommandCase("help|h")]
        public async Task HelpAsync()
        {
            string message = "To authenticate, type 'todos auth app_pass'.\n" +
                             "To get all todos, type 'todos get'.\n" +
                             "To get single todo, type 'todos get some_name'.\n" +
                             "To create or update todo, type 'todos set some_name some_desc'.\n" +
                             "To delete single todo, type 'todos del some_name'.\n\n" +
                             "PS: You can use shortened commands like below:\n" +
                             "todos -> tds, help -> h, get -> g, set -> s, del -> d";

            await ReplyAsync(message);
        }

        // todos auth app_pass
        [CommandCase("auth", "{password}")]
        public async Task AuthAsync(string password)
        {
            if (password != "123456")
            {
                await ReplyAsync("Invalid password!");
            }
            else if (UserTodoItems.ContainsKey(Context.ChatId))
            {
                await ReplyAsync("You are authenticated.");
            }
            else
            {
                UserTodoItems.Add(Context.ChatId, new Dictionary<string, string>());

                await ReplyAsync("You are authenticated.");
            }
        }

        // todos get
        // todos g
        // todos get some_name
        // todos g some_name
        [CommandCase("get|g")]
        [CommandCase("get|g", "{name}")]
        public async Task GetAsync(string name = null)
        {
            IDictionary<string, string> todos = UserTodoItems[Context.ChatId];

            if (!todos.Any())
            {
                await ReplyAsync("There are no todos.");

                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                todos.Keys.ToList().ForEach(async (x) =>
                {
                    await ReplyAsync($"{x} = {todos[x]}");
                });
            }
            else if (!todos.ContainsKey(name))
            {
                await ReplyAsync("The todo is not found.");
            }
            else
            {
                await ReplyAsync(todos[name]);
            }
        }

        // todos set some_name some_desc
        // todos s some_name some_desc
        [CommandCase("set|s", "{name}", "{description}")]
        public async Task SetAsync(string name, string description)
        {
            IDictionary<string, string> todos = UserTodoItems[Context.ChatId];

            if (todos.ContainsKey(name))
            {
                todos[name] = description;

                await ReplyAsync("The todo is updated.");
            }
            else
            {
                todos.Add(name, description);

                await ReplyAsync("The todo is added.");
            }
        }

        // todos del some_name
        // todos d some_name
        [CommandCase("del|d", "{name}")]
        public async Task DelAsync(string name)
        {
            IDictionary<string, string> todos = UserTodoItems[Context.ChatId];

            if (todos.ContainsKey(name))
            {
                todos.Remove(name);
            }

            await ReplyAsync("The todo is deleted.");
        }
    }
}