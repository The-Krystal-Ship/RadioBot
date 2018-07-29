using Discord;
using Discord.Audio;
using Discord.Commands;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Services
{
    public class RadioService : IRadioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients;

        private Process _playbackProcess;

        private readonly IStreamService _streamService;
        private readonly IConfiguration _config;
        private readonly ILogger<RadioService> _logger;
        private bool _isPlaying;

        public RadioService(IStreamService streamService, IConfiguration config, ILogger<RadioService> logger)
        {
            _audioClients = new ConcurrentDictionary<ulong, IAudioClient>();

            _streamService = streamService;
            _config = config;
            _logger = logger;

            AppDomain.CurrentDomain.ProcessExit += async delegate
            {
                foreach (KeyValuePair<ulong, IAudioClient> clients in _audioClients)
                {
                    await clients.Value.StopAsync();
                    clients.Value.Dispose();
                }
            };
        }

        public async Task JoinChannelAsync(IVoiceChannel channel, SocketCommandContext context)
        {
            ulong guildId = context.Guild.Id;

            if (!_audioClients.ContainsKey(guildId))
            {
                IAudioClient audioClient = await channel.ConnectAsync();
                _audioClients.TryAdd(guildId, audioClient);
                _logger.LogInformation($"Connected to {channel.Name}");
            }
        }

        public async Task LeaveChannelAsync(SocketCommandContext context)
        {
            if (!_audioClients.TryGetValue(context.Guild.Id, out IAudioClient audioClient))
            {
                await context.Channel.SendMessageAsync("I'm not connected");
                return;
            }

            try
            {
                await audioClient.StopAsync();
                audioClient.Dispose();
                _isPlaying = false;

                _audioClients.TryRemove(context.Guild.Id, out audioClient);

                _playbackProcess?.CloseMainWindow();
                _playbackProcess?.Close();
                _playbackProcess?.Dispose();
            }
            catch (Exception)
            {
                _logger.LogError("Disconnected from audio client");
            }
        }

        public async Task PlayAsync(string content, SocketCommandContext context)
        {
            if (_isPlaying)
            {
                await context.Channel.SendMessageAsync("Queue is not implemented yet, so you're gonna have to wait");
                return;
            }

            if (!_audioClients.TryGetValue(context.Guild.Id, out IAudioClient client))
            {
                _logger.LogError("Failed to retrieve audio client");
                _isPlaying = false;
                return;
            }

            using (_playbackProcess = _streamService.CreateStream(content))
            using (AudioOutStream discordOutStream = client.CreatePCMStream(AudioApplication.Music))
            {
                _isPlaying = true;

                try
                {
                    _logger.LogInformation("Creating audio stream");

                    Stream ffmpegStream = _playbackProcess.StandardOutput.BaseStream;
                    await ffmpegStream.CopyToAsync(discordOutStream);

                    _logger.LogInformation($"Finished playing: {content}");
                }
                catch (Exception)
                {
                    _logger.LogError("Closed audio steam");
                }
                finally
                {
                    await discordOutStream.FlushAsync();
                    _logger.LogInformation("Flushed stream");
                    _isPlaying = false;
                    _playbackProcess?.CloseMainWindow();
                    _playbackProcess?.Close();
                    _playbackProcess?.Dispose();
                }
            }

            await context.Channel.SendMessageAsync("Finished queue");
        }
    }
}
