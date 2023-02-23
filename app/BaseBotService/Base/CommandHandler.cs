using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace BaseBotService.Base
{
    /// <summary>
    /// The command handler to register with discord.
    /// </summary>
    internal class CommandHandler
    {
        /// <summary>
        /// The prefix that the bot is looking for to execute a command.
        /// </summary>
        // TODO Move the prefix to a configurable setting
        const char Prefix = ';';

        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        // Retrieve client and CommandService instance via ctor
        internal CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        internal async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry assembly and load them.
            // Starting from Discord.NET 2.0, a service provider is required to be passed into
            // the module registration method to inject the required dependencies.
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
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
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(context: context, argPos: argPos, services: null);
        }
    }
}
