using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseBotService.Data.Models;

public class GuildMemberHC
{
    [Key]
    public int Id { get; set; } // Primary key for EF Core

    // Foreign key properties
    public ulong GuildId { get; set; }
    public ulong MemberId { get; set; }

    // Guild-specific properties
    public DateTime LastActive { get; set; }
    public uint ActivityPoints { get; set; }
    public DateTime LastActivityPoint { get; set; }

    // Navigation properties
    [ForeignKey(nameof(GuildId))]
    public GuildHC Guild { get; set; } = default!;

    [ForeignKey(nameof(MemberId))]
    public MemberHC Member { get; set; } = default!;
}
