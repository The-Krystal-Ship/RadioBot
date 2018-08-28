using TheKrystalShip.Configuration;
using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly ILogger<DownloadService> _logger;
        private readonly string _program;
        private readonly string _launchArgs;
        private string _inputArgs;
        private readonly string _outputArgs;

        public DownloadService(ILogger<DownloadService> logger)
        {
            _logger = logger;

            _program = Settings.Instance["Download:Executable"];
            _launchArgs = Settings.Instance["Download:Args"];
            _outputArgs = Settings.Instance["Download:Output"];
        }

        public string Download(string arg)
        {
            _inputArgs = string.Format("\"{0}\"", arg);

            _logger.LogInformation(_inputArgs);

            return ToString();
        }

        public override string ToString()
        {
            string toReturn = $"{_program} {_launchArgs} {_inputArgs} {_outputArgs}";

            _logger.LogInformation(toReturn);

            return toReturn;
        }
    }
}
