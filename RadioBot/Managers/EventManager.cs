using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot
{
    public class EventManager
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<EventManager> _logger;

        public EventManager(DiscordSocketClient client, ILogger<EventManager> logger)
        {
            _client = client;
            _logger = logger;

            _client.Log += ClientLog;
        }

        public Task ClientLog(LogMessage message)
        {
            if (!message.Message.Contains("Malformed Frame"))
            {
                _logger.LogInformation(GetType() + $" ({message.Source})", message.Message);
            }

            return Task.CompletedTask;
        }
    }
}
