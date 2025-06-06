﻿using BaseBotService.Core.Base;

namespace BaseBotService.Commands;

[Group("admin", "Administration of the bot for the current server.")]
[CommandContextType(InteractionContextType.Guild)]
[RequireUserPermission(GuildPermission.Administrator)]
public class AdminModule : BaseModule
{
    public AdminModule(ILogger logger)
    {
        Logger = logger.ForContext<AdminModule>();
    }
}
