﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Reflection;
using System.Threading.Tasks;

using TheKrystalShip.Logging;
using TheKrystalShip.Logging.Extensions;
using TheKrystalShip.RadioBot.Extensions;
using TheKrystalShip.RadioBot.Managers;

namespace TheKrystalShip.RadioBot.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceCollection;
        private readonly ILogger<CommandHandler> _logger;

        public CommandHandler(ref DiscordSocketClient client)
        {
            _client = client;

            _commandService = new CommandService(new CommandServiceConfig()
                {
                    LogLevel = LogSeverity.Debug,
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async
                }
            );

            _commandService.AddModulesAsync(Assembly.GetEntryAssembly()).Wait();

            _commandService.Log += CommandServiceLog;

            _serviceCollection = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddHandlers()
                .AddServices()
                .AddLogger()
                .BuildServiceProvider();

            // Start handlers/services
            _serviceCollection.GetService<EventManager>();
            _logger = _serviceCollection.GetService<ILogger<CommandHandler>>();

            _client.MessageReceived += HandleCommands;
        }

        public Task CommandServiceLog(LogMessage message)
        {
            _logger.LogInformation(GetType().FullName + $" ({message.Source})", message.Message);
            return Task.CompletedTask;
        }

        private async Task HandleCommands(SocketMessage socketMessage)
        {
            SocketUserMessage message = socketMessage as SocketUserMessage;

            if (message is null || message.Author.IsBot)
                return;

            int argPos = 0;
            bool mention = message.HasMentionPrefix(_client.CurrentUser, ref argPos);

            if (!mention)
                return;

            SocketCommandContext context = new SocketCommandContext(_client, message);
            IResult result = await _commandService.ExecuteAsync(context, argPos, _serviceCollection);

            if (!result.IsSuccess)
            {
                _logger.LogError(result.ErrorReason);
            }
        }
    }
}
