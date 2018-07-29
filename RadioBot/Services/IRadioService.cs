using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace TheKrystalShip.RadioBot.Services
{
    public interface IRadioService
    {
        Task JoinChannelAsync(IVoiceChannel channel, SocketCommandContext context);
        Task LeaveChannelAsync(SocketCommandContext context);
        Task PlayAsync(string content, SocketCommandContext context);
    }
}
