using Discord.Commands;
using Discord.WebSocket;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot
{
    public class Tools
    {
        public DiscordSocketClient Client { get; private set; }
        public CommandService CommandService { get; private set; }
        public ILogger<Tools> Logger { get; private set; }
        public AudioPlayer AudioPlayer { get; private set; }

        public Tools(DiscordSocketClient client, CommandService commandService, ILogger<Tools> logger, AudioPlayer audioPlayer)
        {
            Client = client;
            CommandService = commandService;
            Logger = logger;
            AudioPlayer = audioPlayer;
        }
    }
}
