﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using Tomat.Logging;
using Tomat.ServiceFramework;
using Tomat.TomatBot.Content.Configs;

namespace Tomat.TomatBot.Content.Services
{
    public sealed class ConfigService : InitializableService
    {
        public BotConfig Config { get; }

        public static string ConfigPath => "Configs";

        public static JsonSerializerSettings SerializationSettings { get; private set; } = null!;

        public ConfigService(IServiceProvider services) : base(services)
        {
            Directory.CreateDirectory(ConfigPath);

            SerializationSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            Config = new BotConfig();
        }

        public override async Task InitializeAsync()
        {
            Client.JoinedGuild += CreateConfigOnGuildJoin;

            await Logger.TaskLog(new LogMessage(LogSeverity.Debug, "Service", "Initialized ConfigService!"));
        }

        private async Task CreateConfigOnGuildJoin(SocketGuild guild)
        {
            if (Config.Guilds.Any(cGuild => cGuild.associatedId == guild.Id))
                return;

            Config.Guilds.Add(new GuildConfig
            {
                associatedId = guild.Id
            });

            await Task.CompletedTask;
        }
    }
}
