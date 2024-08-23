using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Infrastructure.Achievements;
using Microsoft.EntityFrameworkCore;

namespace BaseBotService.Data;

public class HoneycombDbContext : DbContext
{
    private readonly IEnvironmentService _environmentService;

    public HoneycombDbContext(DbContextOptions<HoneycombDbContext> options, IEnvironmentService environmentService)
        : base(options)
    {
        _environmentService = environmentService;
    }

    public DbSet<MemberHC> Members { get; set; }
    public DbSet<GuildHC> Guilds { get; set; }
    public DbSet<GuildMemberHC> GuildMembers { get; set; }
    public DbSet<AchievementBase> Achievements { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(_environmentService.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // MemberHC Configuration
        modelBuilder.Entity<MemberHC>()
            .HasIndex(m => m.MemberId)
            .IsUnique();

        // GuildHC Configuration
        modelBuilder.Entity<GuildHC>()
            .HasIndex(g => g.GuildId)
            .IsUnique();

        // GuildMemberHC Configuration
        modelBuilder.Entity<GuildMemberHC>()
            .HasIndex(gm => gm.GuildId);
        modelBuilder.Entity<GuildMemberHC>()
            .HasIndex(gm => gm.MemberId);
        modelBuilder.Entity<GuildMemberHC>()
            .HasIndex(gm => new { gm.GuildId, gm.MemberId })
            .IsUnique(); // Ensure a member can only have one entry per guild

        modelBuilder.Entity<GuildMemberHC>()
            .HasOne(gm => gm.Guild)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GuildId);

        modelBuilder.Entity<GuildMemberHC>()
            .HasOne(gm => gm.Member)
            .WithMany() // Assuming a one-to-many relationship (one member can belong to multiple guilds)
            .HasForeignKey(gm => gm.MemberId);

        // AchievementBase Configuration
        modelBuilder.Entity<AchievementBase>()
            .HasIndex(a => a.MemberId);
        modelBuilder.Entity<AchievementBase>()
            .HasIndex(a => a.GuildId);
        modelBuilder.Entity<AchievementBase>()
            .HasIndex(a => new { a.GuildId, a.MemberId });
        modelBuilder.Entity<AchievementBase>()
            .HasIndex(a => a.SourceIdentifier);

        modelBuilder.Entity<AchievementBase>()
            .HasDiscriminator<string>("AchievementType")
            //.HasValue<EasterEventAchievement>("AnotherEvent")
            .HasValue<EasterEventAchievement>("EasterEvent");

        modelBuilder.Entity<AchievementBase>()
            .HasOne<MemberHC>()
            .WithMany()
            .HasForeignKey(a => a.MemberId);

        modelBuilder.Entity<AchievementBase>()
            .HasOne<GuildHC>()
            .WithMany()
            .HasForeignKey(a => a.GuildId);
    }
}
