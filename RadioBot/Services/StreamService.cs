
using System.Diagnostics;
using System.IO;

using TheKrystalShip.Configuration;
using TheKrystalShip.RadioBot.Properties;

namespace TheKrystalShip.RadioBot.Services
{
    public class StreamService : IStreamService
    {
        private readonly IDownloadService _downloadService;
        private readonly IEncodeService _encodeService;

        public StreamService(IDownloadService downloadService, IEncodeService encodeService)
        {
            _downloadService = downloadService;
            _encodeService = encodeService;
        }

        public Process CreateStream(string query)
        {
            string filename = "cmd.exe";
            string arguments = $"/C {_downloadService.Download(query)} | {_encodeService}";

            return Process.Start(new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                UserName = Machine.User,
                PasswordInClearText = Settings.Instance["Local:Password"],
                Domain = Machine.Domain,
                CreateNoWindow = false
            }
            );
        }
    }
}
