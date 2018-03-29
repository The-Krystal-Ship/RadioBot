﻿using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;

using RadioBot.Database.Models;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RadioBot.Services
{
	public class RadioService : Service
    {
		private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels =
			new ConcurrentDictionary<ulong, IAudioClient>();

		public async Task JoinChannel(IVoiceChannel voiceChannel, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			var audioClient = await voiceChannel.ConnectAsync();
			ConnectedChannels.TryAdd(guildId, audioClient);
		}

		public async Task JoinChannel(IAudioClient audioClient, SocketCommandContext context)
		{
			var guildId = context.Guild.Id;
			ConnectedChannels.TryAdd(guildId, audioClient);
		}

		public async Task LeaveChannel(SocketCommandContext Context)
		{
			var guildId = Context.Guild.Id;
			ConnectedChannels.TryGetValue(guildId, out IAudioClient audioClient);

			if (audioClient is null)
			{
				await Context.Channel.SendMessageAsync("Bot is not connected to any Voice Channels");
				return;
			}

			try
			{
				await audioClient.StopAsync();
				audioClient.Dispose();
				ConnectedChannels.TryRemove(Context.Guild.Id, out audioClient);
			}
			catch (Exception)
			{
				Console.WriteLine("Disconnected");
			}
		}

		public async Task SendAudioAsync(SocketGuild guild, ISocketMessageChannel channel, string path)
		{
			var filePath = "";

			if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
			{
				using (var output = CreateStream(filePath).StandardOutput.BaseStream)
				using (var stream = client.CreatePCMStream(AudioApplication.Music))
				{
					try
					{
						await output.CopyToAsync(stream);
					}
					catch (Exception)
					{
						Console.WriteLine("Closed audio stream");
					}
					finally
					{
						await stream.FlushAsync();
					}
				}
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

		public async Task<List<Radio>> GetRadioListAsync()
		{
			return new List<Radio>();
		}
	}
}