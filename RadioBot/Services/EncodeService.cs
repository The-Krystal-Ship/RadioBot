using Microsoft.Extensions.Configuration;

using TheKrystalShip.Logging;

namespace TheKrystalShip.RadioBot.Services
{
    public class EncodeService : IEncodeService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EncodeService> _logger;
        private readonly string _program;
        private readonly string _launchArgs;
        private readonly string _inputArgs;
        private readonly string _outputArgs;

        public EncodeService(IConfiguration config, ILogger<EncodeService> logger)
        {
            _config = config;
            _logger = logger;

            _program = _config["Encode:Executable"];
            _launchArgs = _config["Encode:Args"];
            _inputArgs = _config["Encode:Input"];
            _outputArgs = _config["Encode:Output"];
        }

        public override string ToString()
        {
            string toReturn = $"{_program} {_launchArgs} {_inputArgs} {_outputArgs}";

            _logger.LogInformation(toReturn);

            return toReturn;
        }
    }
}
