using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;
using System.Reflection;

namespace BaseBotService.Base
{
    /// <summary>
    /// The command handler to register with discord.
    /// </summary>
    public class CommandHandler : ICommandHandler
    {
        /// <summary>
        /// The prefix that the bot is looking for to execute a command.
        /// </summary>
        // TODO Move the prefix to a configurable setting
        const char Prefix = ';';

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ILogger _logger;

        // Retrieve client and CommandService instance via constructor.
        internal CommandHandler(ILogger logger, DiscordSocketClient client, CommandService commands)
        {
            _logger = logger;
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // Here we discover all of the command modules in the entry assembly and load them.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
            _commands.CommandExecuted += OnCommandExecutedAsync;
            _client.MessageReceived += HandleCommandAsync;

        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(Prefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            string commandName = command.IsSpecified ? command.Value.Name : "A command";
            switch (result)
            {
                case CommandResult cmdResult:
                    if (!string.IsNullOrEmpty(cmdResult.Reason))
                    {
                        var msg = new EmbedBuilder()
                            .WithTitle("An error occurred!")
                            .WithAuthor(commandName)
                            .WithDescription(cmdResult.Reason)
                            .WithColor(Color.Red);
                        await context.Channel.SendMessageAsync(embed: msg.Build());
                    }
                    break;
                default:
                    if (!string.IsNullOrEmpty(result.ErrorReason))
                    {
                        var msg = new EmbedBuilder()
                            .WithTitle("An error occurred!")
                            .WithAuthor(commandName)
                            .WithDescription(result.ErrorReason)
                            .WithColor(Color.Red);
                        await context.Channel.SendMessageAsync(embed: msg.Build());

                    }
                    break;
            }

            _logger.Write(LogEventLevel.Information, $"{commandName} was executed at {DateTime.UtcNow}.");
        }
    }
}
