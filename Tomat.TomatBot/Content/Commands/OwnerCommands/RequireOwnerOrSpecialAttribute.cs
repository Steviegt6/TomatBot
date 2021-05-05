﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Tomat.TomatBot.Content.Commands.OwnerCommands
{
    public class RequireOwnerOrSpecialAttribute : PreconditionAttribute
    {
        private const ulong StachelID = 442639987180306432;

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
        {
            IApplication? application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);

            // Change this if the hoster changes
            if (context.User.Id == application.Owner.Id || context.User.Id == StachelID)
                return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError(ErrorMessage ??
                                                "Command can only be run by **the owner of the bot** or `TheStachelfisch#0395`.");
        }
    }
}