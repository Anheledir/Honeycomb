using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;

namespace BaseBotService.Infrastructure.Achievements;
public class EasterEventAchievement : AchievementBase
{
    public const string Identifier = "0077B1DF-0E12-4E05-91B1-05A9E9E88248";
    public const string TranslationKey = "achievement-event-easter";

    public EasterEventAchievement(ILogger logger, ITranslationService translationService, IDateTimeProvider dateTimeProvider)
    {
        EventAttributes = TranslationHelper.Arguments("year", dateTimeProvider.GetCurrentUtcDate().Year);
        Name = translationService.GetString(TranslationKey, EventAttributes);
        Description = translationService.GetAttrString(TranslationKey, "description", EventAttributes);
        Emoji = translationService.GetAttrString(TranslationKey, "emoji");
        ImageUrl = translationService.GetAttrString(TranslationKey, "image");
        Points = 10;
        SourceIdentifier = new Guid(Identifier);

        logger.ForContext<EasterEventAchievement>().Debug("EasterEventAchievement created.");
    }
}