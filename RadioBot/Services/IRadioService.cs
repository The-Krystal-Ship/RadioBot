using Discord;
using Discord.Commands;

using System.Threading.Tasks;

namespace RadioBot.Services
{
    public interface IRadioService
    {
        Task JoinChannelAsync(IVoiceChannel channel, SocketCommandContext context);
        Task LeaveChannel(SocketCommandContext context);
        Task PlayAsync(string content, SocketCommandContext context);
    }
}