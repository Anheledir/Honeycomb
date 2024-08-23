using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;

namespace BaseBotService.Infrastructure.Achievements;

public class EasterEventAchievement : AchievementBase
{
    public static new string Identifier => "0077B1DF-0E12-4E05-91B1-05A9E9E88248";
    public static new string TranslationKey => "achievement-event-easter";

    internal EasterEventAchievement() { }

    public EasterEventAchievement(ILogger logger, ITranslationService translationService, IDateTimeProvider dateTimeProvider)
    {
        // Setting up event-specific attributes
        EventAttr = TranslationHelper.Arguments("year", dateTimeProvider.GetCurrentUtcDate().Year);
        Name = translationService.GetString(TranslationKey, EventAttr);
        Description = translationService.GetAttrString(TranslationKey, "description", EventAttr);
        Emoji = translationService.GetAttrString(TranslationKey, "emoji");
        ImageUrl = translationService.GetAttrString(TranslationKey, "image");
        Points = 10;
        SourceIdentifier = new Guid(Identifier);

        logger.ForContext<EasterEventAchievement>().Debug("EasterEventAchievement created.");
    }
}
