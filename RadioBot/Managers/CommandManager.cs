using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Inquisition.Logging;
using Inquisition.Logging.Extensions;

using Microsoft.Extensions.DependencyInjection;

using RadioBot.Extensions;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RadioBot.Managers
{
    public class CommandManager
    {
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commandService;
		private readonly IServiceProvider _serviceCollection;
        private readonly ILogger<CommandManager> _logger;

		public CommandManager(ref DiscordSocketClient client)
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

			_serviceCollection = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddHandlers()
                .AddServices()
                .AddLogger()
				.BuildServiceProvider();

            // Start handlers/services
            _serviceCollection.GetService<EventManager>();
            _logger = _serviceCollection.GetService<ILogger<CommandManager>>();

            _client.MessageReceived += HandleCommands;
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
