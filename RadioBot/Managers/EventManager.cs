using Discord;
using Discord.WebSocket;

using Inquisition.Logging;

using System.Threading.Tasks;

namespace RadioBot.Managers
{
    public class EventManager
    {
		private readonly DiscordSocketClient _client;
        private readonly ILogger<EventManager> _logger;

		public EventManager(DiscordSocketClient client, ILogger<EventManager> logger)
		{
			_client = client;
            _logger = logger;

			_client.Log += Client_Log;
		}

		private Task Client_Log(LogMessage logMessage)
		{
			if (!logMessage.Message.Contains("OpCode"))
			{
                _logger.LogInformation(GetType().FullName + $" ({logMessage.Source})", logMessage.Message);
			}

			return Task.CompletedTask;
		}
	}
}
