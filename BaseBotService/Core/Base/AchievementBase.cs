using BaseBotService.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseBotService.Core.Base;

public abstract class AchievementBase
{
    public const string Identifier = "00000000-0000-0000-0000-000000000000";
    public const string TranslationKey = "achievement";


    [Key]
    public int Id { get; set; } // EF Core requires a primary key, so we'll use an Id field.

    public ulong MemberId { get; set; }
    public ulong? GuildId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Emoji { get; set; } = null!;
    public int Points { get; set; }
    public string? ImageUrl { get; set; }
    public Guid SourceIdentifier { get; set; }
    protected Dictionary<string, object>? EventAttr;

    protected bool IsGlobal => GuildId == null;

    // Navigation properties
    [ForeignKey(nameof(MemberId))]
    public virtual MemberHC Member { get; set; } = null!; // Assuming a member is always present

    [ForeignKey(nameof(GuildId))]
    public virtual GuildHC? Guild { get; set; } // Nullable because some achievements might be global (not tied to a guild)
}
