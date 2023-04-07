using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;

namespace BaseBotService.Infrastructure.Achievements;
public class EasterEventAchievement : HCAchievementBase
{
    public IDateTimeProvider DateTimeProvider { get; set; } = null!;

    public const string Identifier = "0077B1DF-0E12-4E05-91B1-05A9E9E88248";
    public const string TranslationKey = "achievement-event-easter";

    public EasterEventAchievement()
    {
        EventAttributes = TranslationHelper.Arguments("year", DateTimeProvider.GetCurrentUtcDate().Year);
        Name = TranslationService.GetString(TranslationKey, EventAttributes);
        Description = TranslationService.GetAttrString(TranslationKey, "description", EventAttributes);
        Emoji = TranslationService.GetAttrString(TranslationKey, "emoji");
        ImageUrl = TranslationService.GetAttrString(TranslationKey, "image");
        Points = 10;
        SourceIdentifier = new Guid(Identifier);
    }
}