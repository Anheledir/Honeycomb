using Serilog.Events;

namespace BaseBotService.Utilities.Extensions;

public static class DiscordExtensions
{
    /// <summary>
    /// Map a discord log-level to the corresponding serilog log-level.
    /// </summary>
    /// <param name="logMessage">The origin discord log-message.</param>
    /// <returns>The mapped serilog log-severity.</returns>
    public static LogEventLevel GetSerilogSeverity(this LogMessage logMessage)
    {
        return logMessage.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Verbose,
        };
    }

    public static EmbedBuilder WithFieldIf(this EmbedBuilder builder, bool condition, EmbedFieldBuilder item)
    {
        if (condition)
        {
            _ = builder.WithFields(item);
        }
        return builder;
    }

    public static SelectMenuBuilder AddOptionsFromEnum<T>(
        this SelectMenuBuilder builder,
        int currentValue,
        Func<T, string> getLabel
    ) where T : Enum
    {
        bool isFlagsEnum = typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;

        foreach (T value in Enum.GetValues(typeof(T)).Cast<T>())
        {
            string label = getLabel(value).ExtractEmoji(out Emoji emote);
            SelectMenuOptionBuilder option = new SelectMenuOptionBuilder()
                .WithLabel(label)
                .WithValue(Convert.ToInt32(value).ToString());

            if (isFlagsEnum)
            {
                int intValue = Convert.ToInt32(value);
                option.WithDefault(
                    (currentValue & intValue) == intValue
                    && (
                        (intValue > 0 && currentValue != 0)
                        || (intValue == 0 && currentValue == 0)
                        )
                    );
            }
            else
            {
                option.WithDefault(currentValue == Convert.ToInt32(value));
            }

            option.WithEmote(emote);
            _ = builder.AddOption(option);
        }
        return builder;
    }

    /// <summary>
    /// This extension method makes it easier to modify specific text inputs in a modal after it is created.
    /// </summary>
    /// <example>
    /// <code>
    /// await ctx.Interaction.RespondWithModalAsync<ChatTriggerModal>($"chat_trigger_edit:{id},{regex}", null, x => x
    ///    .WithTitle("Chat Trigger Edit")
    ///    .UpdateTextInput("key", x => x.Value = trigger.Trigger)
    ///    .UpdateTextInput("message", x => x.Value = trigger.Response))
    ///    .ConfigureAwait(false);
    /// </code>
    /// </example>
    public static ModalBuilder UpdateTextInput(this ModalBuilder modal, string customId, Action<TextInputBuilder> input)
    {
        var components = modal.Components.ActionRows.SelectMany(r => r.Components).OfType<TextInputComponent>();
        var component = components.First(c => c.CustomId == customId);

        var builder = new TextInputBuilder
        {
            CustomId = customId,
            Label = component.Label,
            MaxLength = component.MaxLength,
            MinLength = component.MinLength,
            Placeholder = component.Placeholder,
            Required = component.Required,
            Style = component.Style,
            Value = component.Value
        };

        input(builder);

        foreach (var row in modal.Components.ActionRows.Where(row => row.Components.Any(c => c.CustomId == customId)))
        {
            row.Components.RemoveAll(c => c.CustomId == customId);
            row.AddComponent(builder.Build());
        }

        return modal;
    }
}