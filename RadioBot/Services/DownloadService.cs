using Microsoft.Extensions.Configuration;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DownloadService> _logger;
        private readonly string _program;
        private readonly string _launchArgs;
        private string _inputArgs;
        private readonly string _outputArgs;

        public DownloadService(IConfiguration config, ILogger<DownloadService> logger)
        {
            _config = config;
            _logger = logger;

            _program = _config["Download:Executable"];
            _launchArgs = _config["Download:Args"];
            _outputArgs = _config["Download:Output"];
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
