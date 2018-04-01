﻿using Discord;
using Discord.Audio;
using Discord.Commands;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RadioBot.Services
{
	public class RadioService
    {
		// Stores each guild's audio client
		private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels;

		public RadioService()
		{
			ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
		}

		public async Task JoinChannel(IAudioClient audioClient, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			if (!ConnectedChannels.ContainsKey(guildId))
			{
				ConnectedChannels.TryAdd(guildId, audioClient);
			}
		}

		public async Task LeaveChannel(SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			ConnectedChannels.TryGetValue(guildId, out IAudioClient audioClient);

			if (audioClient is null)
			{
				await context.Channel.SendMessageAsync("Bot is not connected to any Voice Channels");
				return;
			}

			try
			{
				await audioClient.StopAsync();
				audioClient.Dispose();
				ConnectedChannels.TryRemove(context.Guild.Id, out audioClient);
			}
			catch (Exception)
			{
				Console.WriteLine("Disconnected");
			}
		}

		public async Task PlayAsync(string content, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			if (ConnectedChannels.TryGetValue(guildId, out IAudioClient client))
			{
				// Magic happens here
				using (var output = CreateStream(content).StandardOutput.BaseStream)
				using (var stream = client.CreatePCMStream(AudioApplication.Music))
				{
					try
					{
						await output.CopyToAsync(stream);
					}
					catch (Exception e)
					{
						Console.WriteLine(new LogMessage(LogSeverity.Error, "RadioService", "Closed audio stream"));
					}
					finally
					{
						await stream.FlushAsync();
					}
				}
			}
			else
			{
				Console.WriteLine(new LogMessage(LogSeverity.Error, "RadioService", "Failed to retrieve AudioClient"));
			}
		}

		private Process CreateStream(string path)
		{
			return Process.Start(new ProcessStartInfo
			{
				FileName = "Programs/ffmpeg.exe",
				Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
		}
	}
}
