using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Reflection;
using System.Threading.Tasks;

using TheKrystalShip.Logging;
using TheKrystalShip.Logging.Extensions;

namespace TheKrystalShip.RadioBot
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
                    LogLevel = LogSeverity.Info,
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
                .AddTools()
                .BuildServiceProvider();

            _serviceCollection.GetService<EventManager>();
            _logger = _serviceCollection.GetService<ILogger<CommandHandler>>();

            _client.MessageReceived += HandleCommands;
        }

        public Task CommandServiceLog(LogMessage message)
        {
            if (message.Exception is null)
            {
                _logger.LogInformation(GetType().Name + $" ({message.Source})", message.Message);
            }
            else
            {
                if (message.Exception.InnerException != null)
                {
                    _logger.LogError(message.Exception.InnerException);
                }

                _logger.LogError(message.Exception);
            }
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
