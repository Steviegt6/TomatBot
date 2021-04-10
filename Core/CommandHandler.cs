﻿using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TomatBot.Core.Content.Embeds;

namespace TomatBot.Core
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        internal async Task InstallCommandsAsync()
        {
            // Hook a method to the MessageReceived event, allowing us to detect and respond to messages
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += HandleError;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage msg) || message.Author.IsBot || message.Author.IsWebhook)
                return;

            int argPos = 0;
            if (msg.HasStringPrefix(BotStartup.Prefix, ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                await _commands.ExecuteAsync(new SocketCommandContext(_client, msg), argPos, null);
        }

        private static async Task HandleError(Optional<CommandInfo> info, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                BaseEmbed embed = new(context.User)
                {
                    Title = $"encountered error: {result.Error}",
                    Description = result.ErrorReason
                };

                await context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }
    }
}