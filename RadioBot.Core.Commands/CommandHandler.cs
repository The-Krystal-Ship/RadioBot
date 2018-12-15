using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System.Reflection;
using System.Threading.Tasks;

using TheKrystalShip.DependencyInjection;
using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Core.Commands
{
    public class CommandHandler : CommandService
    {
        private readonly ILogger<CommandHandler> _logger;
        private DiscordSocketClient _client;

        public CommandHandler() : this(new CommandServiceConfig()
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
            DefaultRunMode = RunMode.Async
        })
        {

        }

        public CommandHandler(CommandServiceConfig config) : base(config)
        {
            _logger = new Logger<CommandHandler>();

            AddModulesAsync(Assembly.GetAssembly(typeof(Modules.Module))).Wait();

            Log += OnLogAsync;
            CommandExecuted += OnCommandExecutedAsync;
        }

        public void SetClient(DiscordSocketClient client)
        {
            _client = client;
            _client.Log += OnLogAsync;
        }

        private async Task OnCommandExecutedAsync(CommandInfo command, ICommandContext context, IResult result)
        {
            await Task.CompletedTask;
        }

        public Task OnLogAsync(LogMessage message)
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

        public async Task OnClientMessageRecievedAsync(SocketMessage socketMessage)
        {
            SocketUserMessage message = socketMessage as SocketUserMessage;

            if (message is null || message.Author.IsBot)
                return;

            int argPos = 0;
            bool mention = message.HasMentionPrefix(_client.CurrentUser, ref argPos);

            if (!mention)
                return;

            SocketCommandContext context = new SocketCommandContext(_client, message);
            IResult result = await ExecuteAsync(context, argPos, Container.GetServiceProvider());

            if (!result.IsSuccess)
            {
                _logger.LogError(result.ErrorReason);
            }
        }
    }
}
