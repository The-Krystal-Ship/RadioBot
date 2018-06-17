using Discord;
using Discord.Audio;
using Discord.Commands;

using Inquisition.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RadioBot.Services
{
    public class RadioService
    {
		private readonly ConcurrentDictionary<ulong, IAudioClient> _audioClients;
        private readonly ILogger<RadioService> _logger;
        private bool _isPlaying;

		public RadioService(ILogger<RadioService> logger)
		{
			_audioClients = new ConcurrentDictionary<ulong, IAudioClient>();
            _logger = logger;

            // Disconnect from all voice channels before exiting
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
            ulong guildId = context.Guild.Id;

			if (_audioClients.TryGetValue(guildId, out IAudioClient audioClient))
			{
				try
				{
					await audioClient.StopAsync();
					audioClient.Dispose();
				}
				catch (Exception e)
				{
                    _logger.LogError("Disconnected from audio client", e.Message);
				}

				_audioClients.TryRemove(context.Guild.Id, out audioClient);
			}
			else
			{
				await context.Channel.SendMessageAsync("I'm not connected");
			}
		}

		public async Task PlayAsync(string content, SocketCommandContext context)
		{
            ulong guildId = context.Guild.Id;

            if (_isPlaying)
            {
                await context.Channel.SendMessageAsync("Queue is not implemented yet, so you're gonna have to wait");
                return;
            }

			if (_audioClients.TryGetValue(guildId, out IAudioClient client))
			{
                _isPlaying = true;

				// Magic happens here
				using (Process ffmpegProcess = CreateNetworkStream(content))
				using (AudioOutStream discordOutStream = client.CreatePCMStream(AudioApplication.Music))
				{
					try
					{
                        _logger.LogInformation("Creating audio stream");

						ffmpegProcess.ErrorDataReceived += (sender, args) => {
                            _logger.LogInformation(args.Data);
						};

						var ffmpegStream = ffmpegProcess.StandardOutput.BaseStream;
						await ffmpegStream.CopyToAsync(discordOutStream);
                        _logger.LogInformation("Finished streaming");
                        _isPlaying = false;
					}
					catch (Exception e)
					{
                        _logger.LogError("Closed audio steam", e.Message);
					}
					finally
					{
						await discordOutStream.FlushAsync();
                        _logger.LogInformation("Flushed stream");
                        _isPlaying = false;
					}
				}
			}
			else
			{
                _logger.LogError("Failed to retrieve audio client");
                _isPlaying = false;
			}
		}

		private Process CreateStream(string path)
		{
			string command = $"-hide_banner -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1";

			return Process.Start(new ProcessStartInfo()
				{
					FileName = "ffmpeg.exe",
					Arguments = command,
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			);
		}

        private Process CreateNetworkStream(string path)
        {
            string command = $"/C youtube-dl.exe -q -i --default-search ytsearch -o - \"{path}\" | ffmpeg.exe -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1";

            return Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                }
            );
        }
    }
}
