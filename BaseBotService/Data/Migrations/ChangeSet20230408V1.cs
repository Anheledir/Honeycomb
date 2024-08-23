using BaseBotService.Core.Base;
using BaseBotService.Data.Extensions;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;

namespace BaseBotService.Data.Migrations;

public class ChangeSet20230408V1 : IMigrationChangeset
{
    private readonly ILogger _logger;
    private readonly Lazy<ILiteCollection<MemberHC>> _memberHC;
    private readonly Lazy<ILiteCollection<GuildHC>> _guildHC;
    private readonly Lazy<ILiteCollection<GuildMemberHC>> _guildMemberHC;

    public int Version => 0;

    public ChangeSet20230408V1(
        ILogger logger,
        Lazy<ILiteCollection<MemberHC>> memberHC,
        Lazy<ILiteCollection<GuildHC>> guildHC,
        Lazy<ILiteCollection<GuildMemberHC>> guildMemberHC)
    {
        _logger = logger;
        _logger.Debug("ChangeSet20230408V1 constructor called.");

        _memberHC = memberHC;
        _guildHC = guildHC;
        _guildMemberHC = guildMemberHC;
    }

    public bool ApplyChangeset(ILiteDatabase db, int dbVersion)
    {
        if (dbVersion != Version)
        {
            _logger.Fatal($"The Migration Changeset '{nameof(ChangeSet20230408V1)}' is supposed to run on Database version '{Version}', but it was '{dbVersion}'!");
            return false;
        }

        try
        {
            _logger.Debug("Starting migration changeset for version 0.");

            // Now access the dependencies when they are actually needed
            var memberCollection = _memberHC.Value;
            var guildCollection = _guildHC.Value;
            var guildMemberCollection = _guildMemberHC.Value;

            // Step 1: Initialize the modified MemberHC property 'Achievements'.
            InitializeMemberAchievements(memberCollection);

            // Step 2: Populate the new GuildHC collection with default entries for all Guilds in 'GuildMemberHC'.
            PopulateGuildHCCollection(guildCollection, guildMemberCollection);

            // Step 3: Populate the new GuildHC property 'GuildMembers'.
            PopulateGuildMembers(guildCollection, guildMemberCollection);

            _logger.Information("Migration changeset for version 0 applied successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to apply Changeset 20230408V1");
            return false;
        }
    }

    private void InitializeMemberAchievements(ILiteCollection<MemberHC> memberCollection)
    {
        _logger.Debug("Initializing the modified MemberHC property 'Achievements'.");
        int memberCount = 0;

        var members = memberCollection.FindAll().ToList();
        _logger.Debug($"Retrieved {members.Count} members.");

        foreach (var batch in members.Batch(100))
        {
            _logger.Debug("Processing a new batch of members.");
            foreach (var member in batch)
            {
                if (member == null) continue;

                member.Achievements = new List<AchievementBase>();
                memberCount++;
            }
            _logger.Debug($"Processed batch of 100 members. Total processed: {memberCount}");
        }

        _logger.Debug($"Total members processed: {memberCount}.");
    }

    private void PopulateGuildHCCollection(ILiteCollection<GuildHC> guildCollection, ILiteCollection<GuildMemberHC> guildMemberCollection)
    {
        _logger.Debug("Populating the new GuildHC collection with default entries for all Guilds in 'GuildMemberHC'.");
        int guildCount = 0;

        var guildIds = guildMemberCollection.FindAll().Select(gm => gm.GuildId).Distinct().ToList();
        _logger.Debug($"Retrieved {guildIds.Count} distinct guild IDs.");

        foreach (var batch in guildIds.Batch(100))
        {
            _logger.Debug("Processing a new batch of guild IDs.");
            foreach (var guildId in batch)
            {
                _ = guildCollection.Insert(new GuildHC() { GuildId = guildId, Members = new List<GuildMemberHC>() });
                guildCount++;
            }
            _logger.Debug($"Inserted batch of 100 guilds into GuildHC. Total inserted: {guildCount}");
        }

        _logger.Debug($"Total guilds inserted: {guildCount}.");
    }

    private void PopulateGuildMembers(ILiteCollection<GuildHC> guildCollection, ILiteCollection<GuildMemberHC> guildMemberCollection)
    {
        _logger.Debug("Populating the new GuildHC property 'GuildMembers'.");
        int guildMemberCount = 0;

        var guilds = guildCollection.FindAll().ToList();
        _logger.Debug($"Retrieved {guilds.Count} guilds from GuildHC.");

        foreach (var batch in guilds.Batch(100))
        {
            _logger.Debug("Processing a new batch of guilds.");
            foreach (var guild in batch)
            {
                if (guild == null) continue;

                guild.Members = guildMemberCollection.Find(d => d.GuildId == guild.GuildId).ToList();
                guildCollection.Update(guild);
                guildMemberCount++;
            }
            _logger.Debug($"Updated batch of 100 guilds with their members. Total updated: {guildMemberCount}");
        }

        _logger.Debug($"Total guilds updated: {guildMemberCount}.");
    }
}

