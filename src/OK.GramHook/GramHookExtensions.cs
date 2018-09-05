using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OK.GramHook.Builders;
using OK.GramHook.Handlers;
using OK.GramHook.Services;
using System;
using System.Reflection;

namespace OK.GramHook
{
    public static class GramHookExtensions
    {
        public static IServiceCollection AddGramHook(this IServiceCollection services, Action<GramHookOptions> options)
        {
            GramHookOptions gramHookOptions = new GramHookOptions();

            options.Invoke(gramHookOptions);

            services.AddSingleton<ICommandResolver, CommandResolver>();
            services.AddTransient<IMessageService>((sp) => new MessageService(gramHookOptions.BotToken));
            services.AddTransient<ICommandHandler, CommandHandler>();

            return services;
        }

        public static IApplicationBuilder UseGramHook(this IApplicationBuilder app, string path)
        {
            ICommandHandler commandHandler = app.ApplicationServices.GetService<ICommandHandler>();
            ICommandResolver commandResolver = app.ApplicationServices.GetService<ICommandResolver>();

            commandResolver.Register(Assembly.GetCallingAssembly());

            app.Map(path, _app =>
            {
                _app.Run(commandHandler.HandleAsync);
            });

            return app;
        }
    }
}