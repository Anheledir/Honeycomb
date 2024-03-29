﻿using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using LiteDB;
using BaseBotService.Core.Base;

namespace BaseBotService.Data.Migrations;
public class ChangeSet20230408V1 : IMigrationChangeset
{
    private readonly ILogger _logger;
    private readonly ILiteCollection<MemberHC> _memberHC;
    private readonly ILiteCollection<GuildHC> _guildHC;
    private readonly ILiteCollection<GuildMemberHC> _guildMemberHC;

    public int Version => 0;

    public ChangeSet20230408V1(ILogger logger, ILiteCollection<MemberHC> memberHC, ILiteCollection<GuildHC> guildHC, ILiteCollection<GuildMemberHC> guildMemberHC)
    {
        _logger = logger.ForContext<ChangeSet20230408V1>();
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
            _logger.Debug("Initializing the modified MemberHC property 'Achievements'.");
            foreach (MemberHC? member in _memberHC.FindAll().ToList())
            {
                if (member == null) continue;

                member.Achievements = new List<AchievementBase>();
            }

            _logger.Debug("Populating the new GuildHC collection with default entries for all Guilds in 'GuildMemberHC'.");
            foreach (ulong guildId in _guildMemberHC.FindAll().Select(gm => gm.GuildId).Distinct().ToList())
            {
                _ = _guildHC.Insert(new GuildHC() { GuildId = guildId, Members = new List<GuildMemberHC>() });
            }

            _logger.Debug("Populating the new GuildHC property 'GuildMembers'.");
            foreach (GuildHC? guild in _guildHC.FindAll().ToList())
            {
                if (guild == null) continue;

                guild.Members = _guildMemberHC.Find(d => d.GuildId == guild.GuildId).ToList();
                _guildHC.Update(guild);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to apply Changeset 20230408V1");
            return false;
        }
    }
}
