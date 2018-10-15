using Discord;
using Discord.Audio;
using Discord.Commands;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot
{
    public class RadioService : IRadioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients;
        private readonly AudioPlayer _audioPlayer;

        private SocketCommandContext _context;
        private readonly ILogger<RadioService> _logger;

        public RadioService(AudioPlayer audioPlayer, ILogger<RadioService> logger)
        {
            _audioClients = new ConcurrentDictionary<ulong, IAudioClient>();
            _audioPlayer = audioPlayer;

            _logger = logger;

            _logger.LogInformation($"Started {GetType().Name}");

            AppDomain.CurrentDomain.ProcessExit += async delegate
            {
                foreach (KeyValuePair<ulong, IAudioClient> clients in _audioClients)
                {
                    await clients.Value.StopAsync();
                    clients.Value.Dispose();
                }
            };
        }

        public void SetContext(SocketCommandContext context)
        {
            _context = context;
        }

        public async Task JoinChannelAsync(IVoiceChannel channel)
        {
            ulong guildId = _context.Guild.Id;

            if (!_audioClients.ContainsKey(guildId))
            {
                IAudioClient audioClient = await channel.ConnectAsync();
                _audioClients.TryAdd(guildId, audioClient);
                _logger.LogInformation($"Connected to {channel.Name}");
            }
        }

        public async Task LeaveChannelAsync()
        {
            if (!_audioClients.TryGetValue(_context.Guild.Id, out IAudioClient client))
            {
                await _context.Channel.SendMessageAsync("I'm not connected");
                return;
            }

            _audioPlayer.Stop();

            await _context.Channel.SendMessageAsync("Disconnected from voice channel");

            _audioClients.TryRemove(_context.Guild.Id, out client);
            await client?.StopAsync();
            client?.Dispose();
        }

        public async Task PlayAsync(string content)
        {
            if (!_audioClients.TryGetValue(_context.Guild.Id, out IAudioClient client))
            {
                _logger.LogError("Failed to retrieve audio client");
                return;
            }

            await _audioPlayer.PlayAsync(client, content);

            await _context.Channel.SendMessageAsync("Finished queue");

            _audioClients.TryRemove(_context.Guild.Id, out client);
            await client?.StopAsync();
            client?.Dispose();
        }

        public void Pause() => _audioPlayer.Pause();
        public void Resume() => _audioPlayer.Resume();
        public void SetVolume(float volume) => _audioPlayer.SetVolume(volume);
    }
}
