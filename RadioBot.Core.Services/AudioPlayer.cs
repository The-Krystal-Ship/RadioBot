using Discord.Audio;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Core.Services
{
    public class AudioPlayer
    {
        public bool IsRunning { get; private set; } = false;
        public bool IsPlaying { get; private set; } = false;
        public float Volume { get; set; } = 1.0f;
        public Process Process { get; private set; } = null;
        public Stream Stream { get; private set; } = null;
        
        private readonly ILogger<AudioPlayer> _logger;
        private readonly int _blockSize = 3840;

        public AudioPlayer()
        {
            _logger = new Logger<AudioPlayer>();
        }

        public async Task PlayAsync(IAudioClient client, string query)
        {
            IsRunning = true;
            Process = CreateStream(query);
            Stream = client.CreatePCMStream(AudioApplication.Music);
            IsPlaying = true;

            while (true)
            {
                if (Process is null || Process.HasExited)
                    break;

                if (Stream is null)
                    break;

                if (!IsPlaying)
                    continue;

                int blockSize = _blockSize;
                byte[] buffer = new byte[blockSize];
                int byteCount;
                byteCount = await Process.StandardOutput.BaseStream.ReadAsync(buffer, 0, blockSize);

                if (byteCount <= 0)
                    break;

                try
                {
                    await Stream.WriteAsync(ScaleVolumeSafeAllocateBuffers(buffer, Volume), 0, byteCount);
                }
                catch (Exception e)
                {
                    _logger.LogError(e);
                    break;
                }
            }

            try
            {
                if (Process != null && !Process.HasExited)
                {
                    Process.Kill();
                    Process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }

            try
            {
                if (Stream != null)
                    Stream.FlushAsync().Wait();
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }

            Process = null;
            Stream = null;
            IsPlaying = false;
            IsRunning = false;
        }

        private byte[] ScaleVolumeSafeAllocateBuffers(byte[] audioSamples, float volume)
        {
            if (audioSamples == null)
                return null;

            if (audioSamples.Length % 2 != 0)
                return null;

            if (volume < 0.0f || volume > 1.0f)
                return null;

            byte[] output = new byte[audioSamples.Length];

            try
            {
                if (Math.Abs(volume - 1f) < 0.0001f)
                {
                    Buffer.BlockCopy(audioSamples, 0, output, 0, audioSamples.Length);
                    return output;
                }

                int volumeFixed = (int)Math.Round(volume * 65536d);

                for (var i = 0; i < output.Length; i += 2)
                {
                    int sample = (short)((audioSamples[i + 1] << 8) | audioSamples[i]);
                    int processed = (sample * volumeFixed) >> 16;

                    output[i] = (byte)processed;
                    output[i + 1] = (byte)(processed >> 8);
                }

                return output;
            }
            catch (Exception e)
            {
                _logger.LogError(e);
                return null;
            }
        }

        private Process CreateStream(string query)
        {
            return Process.Start(new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C youtube-dl.exe --default-search ytsearch -o - \"{query}\" | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            );
        }

        public void SetVolume(float volume)
        {
            if (volume < 0.0f)
            {
                volume = 0.0f;
            }
            else if (volume > 1.0f)
            {
                volume = 1.0f;
            }

            Volume = volume;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            if (Process != null)
            {
                Process.Kill();
            }
        }
    }
}
