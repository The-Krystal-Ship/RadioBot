using Discord;
using Discord.WebSocket;

using Inquisition.Logging;

using System.Threading.Tasks;

namespace RadioBot.Handlers
{
    public class EventHandler
    {
		private DiscordSocketClient _client;
        private readonly ILogger<EventHandler> _logger;

		public EventHandler(
            DiscordSocketClient client,
            ILogger<EventHandler> logger)
		{
			_client = client;
            _logger = logger;

			_client.Log += Client_Log;
		}

		private Task Client_Log(LogMessage logMessage)
		{
			if (!logMessage.Message.Contains("OpCode"))
			{
                _logger.LogInformation(logMessage.Source, logMessage.Message);
			}

			return Task.CompletedTask;
		}
	}
}
