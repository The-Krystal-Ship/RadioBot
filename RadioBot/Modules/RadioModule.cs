using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot
{
    public class RadioModule : Module
    {
        private readonly IRadioService _radioService;

        public RadioModule(Tools tools, IRadioService radioService) : base(tools)
        {
            _radioService = radioService;
            _radioService.SetContext(Context);
        }

        [Command("join")]
        public async Task JoinVoiceChannelAsync()
        {
            _radioService.SetContext(Context);

            IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if (userVoiceChannel is null)
            {
                await ReplyAsync("You're not in a voice channel");
                return;
            }

            await _radioService.JoinChannelAsync(userVoiceChannel);
        }

        [Command("leave")]
        [Alias("stop", "fuck off")]
        public async Task LeaveAsync()
        {
            _radioService.SetContext(Context);

            await _radioService.LeaveChannelAsync();
        }

        [Command("play")]
        public async Task PlayAsync([Remainder] string content = null)
        {
            _radioService.SetContext(Context);

            if (content is null)
            {
                await ReplyAsync("You need to tell me something to play");
                return;
            }

            IVoiceChannel userVoiceChannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if (userVoiceChannel is null)
            {
                await ReplyAsync("You're not in a voice channel");
                return;
            }

            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("\u25B6"));

            await _radioService.JoinChannelAsync(userVoiceChannel);
            await _radioService.PlayAsync(content);
        }

        [Command("pause")]
        public async Task PausePlaybackAsync()
        {
            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("\u23F8"));

            _radioService.Pause();
        }

        [Command("resume")]
        [Alias("continue")]
        public async Task ResumePlaybackAsync()
        {
            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("\u25B6"));

            _radioService.Resume();
        }

        [Command("volume")]
        public async Task SetVolumeAsync(float volume)
        {
            IUserMessage message = Context.Message;
            await message.AddReactionAsync(new Emoji("👍🏻"));

            _radioService.SetVolume(volume / 100);
        }
    }
}
