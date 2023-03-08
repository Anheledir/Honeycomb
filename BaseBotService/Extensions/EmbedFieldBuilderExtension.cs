using Discord;

namespace BaseBotService.Extensions;
public static class EmbedBuilderExtension
{
    public static EmbedBuilder WithFieldIf(this EmbedBuilder builder, bool condition, EmbedFieldBuilder item)
    {
        if (condition)
        {
            builder.WithFields(item);
        }
        return builder;
    }
}
