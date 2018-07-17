using Discord.WebSocket;

using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Managers
{
    public class EventManager
    {
		private readonly DiscordSocketClient _client;
        private readonly ILogger<EventManager> _logger;

		public EventManager(DiscordSocketClient client, ILogger<EventManager> logger)
		{
			_client = client;
            _logger = logger;

            _client.Log += (message) =>
            {
                if (!message.Message.Contains("OpCode"))
                {
                    _logger.LogInformation(GetType() + $" ({message.Source})", message.Message);
                }

                return Task.CompletedTask;
            };
		}
	}
}
