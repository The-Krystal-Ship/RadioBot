using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;

using TheKrystalShip.DependencyInjection;
using TheKrystalShip.RadioBot.Core.Commands;

namespace TheKrystalShip.RadioBot.Core
{
    public class Bot : DiscordSocketClient
    {
        private readonly CommandHandler _commandHandler;

        public Bot() : this(new DiscordSocketConfig() {LogLevel = LogSeverity.Info, DefaultRetryMode = RetryMode.AlwaysRetry})
        {

        }

        public Bot(DiscordSocketConfig config) : base(config)
        {
            _commandHandler = Container.Get<CommandHandler>();
            _commandHandler.SetClient(this);

            MessageReceived += _commandHandler.OnClientMessageRecievedAsync;
        }

        public async Task InitAsync(string token)
        {
            await LoginAsync(TokenType.Bot, token);
            await StartAsync();
            await SetGameAsync("music 🎶");
        }
    }
}
