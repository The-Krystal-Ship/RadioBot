using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;

using TheKrystalShip.RadioBot.Handlers;
using TheKrystalShip.RadioBot.Properties;

namespace TheKrystalShip.RadioBot
{
    public class Program
    {
		private static string _token;
		private static DiscordSocketClient _client;
		private static CommandHandler _commandHandler;

		public static async Task Main(string[] args)
		{
            Console.Title = "RadioBot";

            _token = Configuration.Get("Bot", "Token");

			_client = new DiscordSocketClient(new DiscordSocketConfig()
				{
                    LogLevel = LogSeverity.Debug,
					DefaultRetryMode = RetryMode.AlwaysRetry,
					ConnectionTimeout = 5000
				}
			);

			_commandHandler = new CommandHandler(ref _client);

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            await _client.SetGameAsync("music 🎶");

            await Task.Delay(-1);
        }
    }
}
