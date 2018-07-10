using Discord;
using Discord.Commands;

using Inquisition.Logging;

using System.Threading.Tasks;

using TheKrystalShip.RadioBot.Services;

namespace TheKrystalShip.RadioBot.Modules
{
    public class RadioModule : ModuleBase<SocketCommandContext>
    {
		private readonly IRadioService _radioService;
        private readonly ILogger<RadioModule> _logger;
        
		public RadioModule(IRadioService radioService, ILogger<RadioModule> logger)
		{
			_radioService = radioService;
            _logger = logger;
		}

		[Command("join")]
		public async Task JoinVoiceChannelAsync()
		{
			// Is user requesting in a voice channel?
			IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

			if (userVoiceChannel is null)
			{
				await ReplyAsync("You're not in a voice channel");
				return;
			}

			// Sent request to service
			await _radioService.JoinChannelAsync(userVoiceChannel, Context);
		}

		[Command("leave")]
		[Alias("stop")]
		public async Task LeaveAsync()
		{
			// Service handles checks on this one
			await _radioService.LeaveChannelAsync(Context);
		}

		[Command("play")]
		public async Task PlayAsync([Remainder] string content = null)
		{
			if (content is null)
			{
				await ReplyAsync("You need to tell me something to play");
				return;
			}

			// Is user requesting in a voice channel?
			IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

			if (userVoiceChannel is null)
			{
				await ReplyAsync("You're not in a voice channel");
				return;
			}

            // Service will only join if not already in the voice channel
			await _radioService.JoinChannelAsync(userVoiceChannel, Context);

			// Send play request to service
			await _radioService.PlayAsync(content, Context);
		}
    }
}
