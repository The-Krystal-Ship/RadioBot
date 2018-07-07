using Discord;
using Discord.Audio;
using Discord.Commands;

using Inquisition.Logging;

using RadioBot.Properties;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RadioBot.Services
{
    public class RadioService : IRadioService
    {
		private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients;
        private Process _process;
        private readonly ILogger<RadioService> _logger;
        private bool _isPlaying;

		public RadioService(ILogger<RadioService> logger)
		{
			_audioClients = new ConcurrentDictionary<ulong, IAudioClient>();
            _logger = logger;
            
            AppDomain.CurrentDomain.ProcessExit += async delegate {
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

		public async Task LeaveChannel(SocketCommandContext context)
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

                _process?.CloseMainWindow();
                _process?.Close();
                _process?.Dispose();
            }
			catch (Exception e)
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
            
            using (Process ffmpegProcess = CreateNetworkStream(content))
            using (AudioOutStream discordOutStream = client.CreatePCMStream(AudioApplication.Music))
            {
                _isPlaying = true;

                try
                {
                    _logger.LogInformation("Creating audio stream");

                    Stream ffmpegStream = ffmpegProcess.StandardOutput.BaseStream;
                    await ffmpegStream.CopyToAsync(discordOutStream);

                    _logger.LogInformation("Finished streaming");
                    await context.Channel.SendMessageAsync("Finished streaming");
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
                }
            }
        }

        private Process CreateStream(string query)
        {
            string filename = "ffmpeg.exe";

            string command = $"-hide_banner -i \"{query}\" -ac 2 -f s16le -ar 48000 pipe:1";

            return Process.Start(new ProcessStartInfo()
                {
                    FileName = filename,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            );
        }

        private Process CreateNetworkStream(string query)
        {
            string filename = "cmd.exe";

            string download = $"/C youtube-dl.exe -q -i --default-search ytsearch -o - \"{query}\"";
            string encode = $"ffmpeg.exe -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1";
            string command = $"{download} | {encode}";

            return _process = Process.Start(new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = command,
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    UserName = Machine.User(),
                    PasswordInClearText = Configuration.Get("Local", "Password"),
                    Domain = Machine.Domain(),
                    CreateNoWindow = false
                }
            );
        }
    }
}
