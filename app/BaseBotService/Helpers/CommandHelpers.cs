using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using Serilog;

namespace BaseBotService.Helpers
{
    public class CommandHelpers
    {
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;

        public CommandHelpers(ILogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task RegisterGlobalCommandsAsync(bool overwrite = false)
        {
            _logger.Information($"Register global application commands (overwrite mode: {overwrite})");

            List<SlashCommandBuilder> commands = new()
            {
                // info module
                new SlashCommandBuilder()
                    .WithName("info")
                    .WithDescription("Returns the basic information of the bot."),

                // users module
                new SlashCommandBuilder()
                    .WithName("user-info")
                    .WithDescription("Returns info about the current user, or the user parameter, if one is passed.")
                    .AddOption("user", ApplicationCommandOptionType.User, "The users who's information you want to see", isRequired: false),
                new SlashCommandBuilder()
                    .WithName("user-roles")
                    .WithDescription("Lists all roles of a user.")
                    .AddOption("user", ApplicationCommandOptionType.User, "The users who's roles you want to be listed", isRequired: false),
            };

            try
            {
                // Create all global application commands
                if (overwrite)
                {
                    List<ApplicationCommandProperties> applicationCommandProperties = new();
                    commands.ForEach(cmd =>
                    {
                        _logger.Information($"Register global command '{cmd.Name}'");
                        applicationCommandProperties.Add(cmd.Build());
                    });
                    _logger.Information($"Bulk overwrite of ({applicationCommandProperties.Count}) global application commands.");
                    _ = await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
                }
                else
                {
                    commands.ForEach(async cmd =>
                    {
                        _logger.Information($"Register global command '{cmd.Name}'");
                        await _client.CreateGlobalApplicationCommandAsync(cmd.Build());
                    });
                }

                _logger.Information($"Finished registering ({commands.Count}) global application commands.");
            }
            catch (HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                _logger.Error(json);
            }

        }
    }
}