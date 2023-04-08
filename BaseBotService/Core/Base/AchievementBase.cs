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

    public static ILiteCollection<AchievementBase> GetServiceRegistration(IServiceProvider services)
    {
        ILiteCollection<AchievementBase> collection = GetServiceRegistration<AchievementBase>(services);
        _ = collection.EnsureIndex(x => x.MemberId);
        _ = collection.EnsureIndex(x => x.GuildId);
        _ = collection.EnsureIndex(x => new { x.GuildId, x.MemberId });
        _ = collection.EnsureIndex(x => x.SourceIdentifier);

        return collection;
    }
}
