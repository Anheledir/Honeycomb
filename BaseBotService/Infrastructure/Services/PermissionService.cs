using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using Discord.WebSocket;

namespace BaseBotService.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ILogger _logger;
        private readonly IGuildRepository _guildRepo;

        public PermissionService(ILogger logger, IGuildRepository guildRepo)
        {
            _logger = logger;
            _guildRepo = guildRepo;
        }

        public bool IsUserAdmin(SocketGuildUser user) => user.GuildPermissions.Administrator;

        public bool IsUserModerator(SocketGuildUser user, SocketRole modRoles) => user.Roles.Contains(modRoles);

        public List<ulong> GetModeratorRoles(SocketGuild guild) => _guildRepo.GetGuild(guild.Id, true).ModeratorRoles;
        public List<ulong> GetArtistRoles(SocketGuild guild) => _guildRepo.GetGuild(guild.Id, true).ArtistRoles;

        public bool CanUserExecuteModeratorCommand(SocketGuildUser? user)
        {
            if (user == null) return false;
            List<ulong> moderatorRoles = GetModeratorRoles(user.Guild);
            return IsUserAdmin(user) || user.Roles.Any(role => moderatorRoles.Contains(role.Id));
        }

        public bool CanUserExecuteArtistCommand(SocketGuildUser? user)
        {
            if (user == null) return false;
            List<ulong> artistRoles = GetArtistRoles(user.Guild);
            return user.Roles.Any(role => artistRoles.Contains(role.Id));
        }

        public void SetModeratorRole(SocketGuild guild, SocketRole role)
        {
            var currentGuild = _guildRepo.GetGuild(guild.Id, true);
            currentGuild.ModeratorRoles.Clear();
            currentGuild.ModeratorRoles.Add(role.Id);
            _guildRepo.UpdateGuild(currentGuild);
        }

        public SocketRole GetModeratorRole(SocketGuild guild)
        {
            var currentGuild = _guildRepo.GetGuild(guild.Id, true);
            return currentGuild.ModeratorRoles.Count > 0 ? guild.GetRole(currentGuild.ModeratorRoles[0]) : null;
        }

        public Task<bool> CanUserExecuteModeratorCommandAsync(SocketGuildUser? user) => Task.Run(() => CanUserExecuteModeratorCommand(user));

        public Task<bool> CanUserExecuteArtistCommandAsync(SocketGuildUser? user) => Task.Run(() => CanUserExecuteArtistCommand(user));

    }
}
