# OK.GramHook
A .Net Standard Wrapper to Manage Commands with Telegram Bot Webhooks.

## How to use
Install the package from NuGet using PowerShell.

```
Install-Package OK.GramHook
```

Or install with DotNet CLI.
```
dotnet add package OK.GramHook
```

Call "AddGramHook" and "UseGramHook" extension methods in Startup file. Don't forget specify your Telegram Bot token and if you want, you can change listening path string.

```c#
using OK.GramHook;

...

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGramHook(options =>
        {
            options.BotToken = "YOUR_BOT_TOKEN";
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseGramHook("/api/webhook");
    }
}
```

Create command class

- Add a class that implements "CommandBase" from OK.GramHook package.

- Add "Command" attribute using command name argument to command class.

- Add case methods for each command combinations using "CommandCase" attribute with command arguments array.

```c#
using OK.GramHook;

...

[Command("ping")]
public class PingCommand : CommandBase
{
    [CommandCase("{name}")]
    public async Task PingAsync(string name)
    {
        await ReplyAsync($"pong {name}");
    }
}
```

## How it works
- When the application starts from Startup file, the library checks all classes that implements `CommandBase` class using System.Reflection.
- The found command classes saved to an internal dictionary with empty command case items.
- For each command classes, the library checks class methods that tagged with `CommandCase` attribute using System.Reflection.
- For each command cases, the library checks attribute arguments has pipe or brackets and it is added a case item to the dictionary that created for the command before.
- OK, all commands are in dictionary now. When a request received to the specified path, the library parses request body and fills user and message informations in `Context` property in `CommandBase`.
- The library separates the requested message text with space char and decides which command case matched.
- Finally, invokes the decided method with specified parameters if exists.

## What can to do
- You can use constructor to inject dependencies. Then you can use it where you want.
    ```c#
    [Command("auth")]
    public class AuthCommand : CommandBase
    {
        private readonly IAuthManager _authManager;

        public AuthCommand(IAuthManager authManager)
        {
            _authManager = authManager;
        }
    }
    ```
- You can reply the message using `ReplyAsync` method with message string in base class `CommandBase`
    ```c#
    [CommandCase]
    public async Task DoAsync()
    {
        await ReplyAsync("Hello! I'm starting your request.");

        // Do something

        await ReplyAsync("Done! I did it.");
    }
    ```
- You can abort the command execution before invoking the command case method using `AbortAsync` method in base class `CommandBase`. The feature is for using in `OnPreExecutionAsync` method.
    ```c#
    [CommandCase]
    public override async Task OnPreExecutionAsync()
    {
        // Do something

        await ReplyAsync("Aborted.");

        await AbortAsync();
    }
    ```
- You can access the sender `MessageId`, `MessageText`, `ChatId`, `Username`, `FirstName`, `LastName` informations with `Context` property in base class `CommandBase`.
    ```c#
    [Command("hello")]
    public class HelloCommand : CommandBase
    {
        [CommandCase]
        public async Task HelloAsync()
        {
            string username = Context.Username;

            await ReplyAsync($"Hello @{username}!");
        }
    }
    ```
- You can specify command aliases in `Command` attribute with pipe char separated name argument.
    ```c#
    [Command("get|g")]
    public class GetCommand : CommandBase
    {

    }
    ```
    With the code above, you can use with `get` or `g` commands.
    
- You can also specify command case aliases in methods using pipe char separated argument.
    ```c#
    [CommandCase("all|a")]
    public async Task GetAllAsync()
    {

    }
    ```
    You can do this using multiple `CommandCase` attribute too. Some combinations can require this way like different parameter counts.
    ```c#
    [CommandCase("all")]
    [CommandCase("a")]
    public async Task GetAllAsync()
    {

    }
    ```
    With the codes above, you can use with `get all` or `get a` commands.

- You can pass the arguments that specified in message to related command case methods. To do this, you should use brackets in `CommandCase` attribute's arguments.

    ```c#
    [CommandCase("{age}")]
    public async Task SetAgeAsync(int age)
    {
        await ReplyAsync($"Your age is updated as {age}");
    }
    ```
- You can do some operations at pre execution or post execution time using `OnPreExecutionAsync` and `OnPostExecutionAsync` methods in base class `CommandBase`.
    ```c#
    public override async Task OnPreExecutionAsync()
    {
        await ReplyAsync("On Pre Execution");

        if (!_userManager.IsUserRegistered(Context.ChatId))
        {
            await ReplyAsync("You are not authorized to do this. Aborted your command.");

            await AbortAsync();
        }
    }

    public override async Task OnPostExecutionAsync()
    {
        await ReplyAsync("On Post Execution");
    }
    ```