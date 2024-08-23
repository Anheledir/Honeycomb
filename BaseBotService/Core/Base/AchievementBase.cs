using LiteDB;

namespace BaseBotService.Core.Base;

public abstract class AchievementBase : ModelBase
{
    protected Dictionary<string, object>? EventAttributes;

    protected bool IsGlobal => GuildId == null;

    public ulong MemberId { get; set; }
    public ulong? GuildId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Emoji { get; set; } = null!;
    public int Points { get; set; }
    public string? ImageUrl { get; set; }
    public Guid SourceIdentifier { get; set; }
    public static string Identifier => "00000000-0000-0000-0000-000000000000";
    public static string TranslationKey => "achievement";

    // Override the EnsureIndexes method to set up indexes on the collection
    protected override void EnsureIndexes<T>(ILiteCollection<T> collection)
    {
        var achievementCollection = collection as ILiteCollection<AchievementBase>;

        achievementCollection?.EnsureIndex(x => x.MemberId);
        achievementCollection?.EnsureIndex(x => x.GuildId);
        achievementCollection?.EnsureIndex(x => new { x.GuildId, x.MemberId });
        achievementCollection?.EnsureIndex(x => x.SourceIdentifier);
    }
}
