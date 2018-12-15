using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot.Core.Services
{
    public interface IRadioService
    {
        Task JoinChannelAsync(IVoiceChannel channel);
        Task LeaveChannelAsync();
        Task PlayAsync(string content);
        void SetContext(SocketCommandContext context);
        void Pause();
        void Resume();
        void SetVolume(float volume);
    }
}
