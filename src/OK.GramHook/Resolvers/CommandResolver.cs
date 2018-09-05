using OK.GramHook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OK.GramHook.Builders
{
    internal class CommandResolver : ICommandResolver
    {
        private IDictionary<string, List<CommandModel>> Commands { get; set; }

        public void Register(Assembly assembly)
        {
            Commands = new Dictionary<string, List<CommandModel>>();

            IEnumerable<Type> types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CommandBase)) &&
                                                                     t.IsAbstract == false &&
                                                                     t.CustomAttributes.Any(a => a.AttributeType == typeof(CommandAttribute)));

            foreach (var type in types)
            {
                var commandAttr = type.GetCustomAttribute<CommandAttribute>(false);
                var methods = type.GetMethods()
                                  .Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(CommandCaseAttribute)));

                foreach (var method in methods)
                {
                    var commandCaseAttrs = method.GetCustomAttributes<CommandCaseAttribute>();

                    foreach (var commandCaseAttr in commandCaseAttrs)
                    {
                        CommandModel command = new CommandModel();

                        command.ArgumentDetails = new List<ArgumentModel>();
                        command.Type = type;
                        command.Method = method;

                        foreach (var commandArg in commandCaseAttr.Arguments)
                        {
                            string parameterName = commandArg.StartsWith("{") && commandArg.EndsWith("}")
                                ? commandArg.TrimStart('{').TrimEnd('}').Replace("?", string.Empty)
                                : null;

                            List<string> availableValues = commandArg.Contains('|')
                                ? commandArg.Split('|').ToList()
                                : new List<string>() { commandArg };

                            command.ArgumentDetails.Add(new ArgumentModel()
                            {
                                ParameterName = parameterName,
                                AvailableValues = availableValues
                            });
                        }

                        List<string> availableCommands = commandAttr.Name.Contains("|")
                            ? commandAttr.Name.Split('|').ToList()
                            : new List<string>() { commandAttr.Name };
                        
                        foreach (var commandValue in availableCommands)
                        {
                            if (Commands.ContainsKey(commandValue))
                            {
                                Commands[commandValue].Add(command);
                            }
                            else
                            {
                                Commands.Add(commandValue, new List<CommandModel>() { command });
                            }
                        }
                    }
                }
            }
        }

        public ResolveModel Resolve(string text)
        {
            List<string> sections = text.Split(' ').ToList();

            string commandName = sections[0];

            if (!Commands.ContainsKey(commandName))
            {
                return null;
            }

            List<string> args = sections.Skip(1).ToList();

            List<CommandModel> commands = Commands[commandName];

            foreach (var command in commands)
            {
                if (args.Count != command.ArgumentDetails.Count)
                {
                    continue;
                }

                bool passCurrentCommand = false;

                IDictionary<string, object> methodParams = new Dictionary<string, object>();

                for (int i = 0; i < args.Count; i++)
                {
                    ArgumentModel argument = command.ArgumentDetails[i];

                    if (!string.IsNullOrEmpty(argument.ParameterName))
                    {
                        methodParams.Add(argument.ParameterName, args[i]);
                    }
                    else
                    {
                        if (!argument.AvailableValues.Contains(args[i]))
                        {
                            passCurrentCommand = true;

                            break;
                        }
                    }
                }

                if (passCurrentCommand)
                {
                    continue;
                }

                return new ResolveModel()
                {
                    CommandType = command.Type,
                    Method = command.Method,
                    Parameters = methodParams
                };
            }

            return null;
        }
    }
}