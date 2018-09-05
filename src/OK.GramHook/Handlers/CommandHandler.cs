using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OK.GramHook.Builders;
using OK.GramHook.Models;
using OK.GramHook.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OK.GramHook.Handlers
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly IMessageService _messageService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandResolver _commandResolver;

        public CommandHandler(IMessageService messageService,
                              IServiceProvider serviceProvider,
                              ICommandResolver commandResolver)
        {
            _messageService = messageService;
            _serviceProvider = serviceProvider;
            _commandResolver = commandResolver;
        }

        public async Task HandleAsync(HttpContext context)
        {
            string bodyString;

            using (var reader = new StreamReader(context.Request.Body))
            {
                bodyString = reader.ReadToEnd();
            }

            BotUpdateModel botUpdate = JsonConvert.DeserializeObject<BotUpdateModel>(bodyString);

            if (botUpdate == null)
            {
                context.Response.StatusCode = 500;

                return;
            }

            string messageId = botUpdate.Message.Id.ToString();
            string messageText = botUpdate.Message.Text.Trim();
            string chatId = botUpdate.Message.Chat.Id.ToString();
            string username = botUpdate.Message.From.Username;
            string firstName = botUpdate.Message.From.FirstName;
            string lastName = botUpdate.Message.From.LastName;

            try
            {
                ResolveModel resolve = _commandResolver.Resolve(messageText);

                if (resolve == null)
                {
                    await _messageService.SendAsync(chatId, "Invalid Command!");

                    context.Response.StatusCode = 200;

                    return;
                }

                CommandBase commandInstance = (CommandBase)CreateInstance(resolve.CommandType);

                CommandContext commandContext = new CommandContext
                {
                    MessageId = messageId,
                    MessageText = messageText,
                    ChatId = chatId,
                    Username = username,
                    FirstName = firstName,
                    LastName = lastName
                };

                commandInstance.Context = commandContext;
                commandInstance.OnReplyAsync = (text) => _messageService.SendAsync(commandContext.ChatId, text);

                await commandInstance.OnPreExecutionAsync();

                if (!commandInstance.IsAborted)
                {
                    InvokeMethod(resolve.Method, commandInstance, resolve.Parameters);
                }

                await commandInstance.OnPostExecutionAsync();
            }
            catch (Exception)
            {
                await _messageService.SendAsync(chatId, "An error occured! Try again.");
            }

            context.Response.StatusCode = 200;
        }

        #region Helpers

        private object CreateInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];

            if (constructor != null)
            {
                object[] args = constructor.GetParameters()
                                           .Select(x => x.ParameterType)
                                           .Select(x => _serviceProvider.GetService(x))
                                           .ToArray();

                return Activator.CreateInstance(type, args);
            }

            return null;
        }

        private void InvokeMethod(MethodBase method, object obj, IDictionary<string, object> namedParameters)
        {
            string[] paramNames = method.GetParameters().Select(p => p.Name).ToArray();
            object[] parameters = new object[paramNames.Length];

            for (int i = 0; i < parameters.Length; ++i)
            {
                parameters[i] = Type.Missing;
            }

            foreach (var item in namedParameters)
            {
                string paramName = item.Key;
                int paramIndex = Array.IndexOf(paramNames, paramName);

                parameters[paramIndex] = item.Value;
            }

            object val = method.Invoke(obj, parameters);

            if (val is Task task)
            {
                task.Wait();
            }
        }

        #endregion
    }
}